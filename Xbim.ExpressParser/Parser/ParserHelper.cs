using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Xbim.Gppg;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Management.Instrumentation;
using Microsoft.CSharp;
using Xbim.ExpressParser.SDAI;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

// ReSharper disable once CheckNamespace
namespace Xbim.ExpressParser
{
    internal partial class Parser
    {
        public SchemaModel Model { get; private set; }

        internal Parser(Scanner lex): base(lex)
        {
            Model = new SchemaModel();
        }

        // ReSharper disable once InconsistentNaming
        private readonly List<Action> ToDoActions = new List<Action>();
        // ReSharper disable once InconsistentNaming
        private readonly List<Action> ToDoPostActions = new List<Action>();

        //do all postprocessing, assignments and other operations
        private void Finish()
        {
            foreach (var action in ToDoActions)
            {
                action();
            }
            foreach (var action in ToDoPostActions)
            {
                action();
            }
            //clear for the next processing/parsing
            ToDoActions.Clear();
            ToDoPostActions.Clear();
        }

        private void CreateEnumeration(string name, List<string> values)
        {
            Model.New((EnumerationType e) =>
            {
                e.Name = name;
                e.Elements = new List<ExpressId>();
                e.Elements.AddRange(values);
            });
        }

        private void CreateSelectType(string name, List<string> selections)
        {
            var select = Model.New((SelectType e) =>
            {
                e.Name = name;
                e.Selections = new List<NamedType>();
            });

            foreach (var selection in selections)
            {
                ToDoActions.Add(() =>
                {
                    var type = Model.Get<NamedType>(n => n.Name == selection).FirstOrDefault();
                    if(type == null)
                        throw new InstanceNotFoundException();
                    select.Selections.Add(type);
                });
            }
        }

        private void CreateEntity(string name, IEnumerable<ValueType> sections)
        {
            var entity = Model.New<EntityDefinition>(e => e.Name = name);
            foreach (var section in sections)
            {
                switch (section.tokVal)
                {
                    case Tokens.UNIQUE:
                        var unique = section.val as List<UniquenessRule>;
                        if (unique != null && unique.Any())
                            foreach (var attribute in unique)
                                attribute.ParentEntity = entity;
                        break;
                    case Tokens.INVERSE:
                        var inverse = section.val as List<InverseAttribute>;
                        if (inverse != null && inverse.Any())
                            foreach (var attribute in inverse)
                                attribute.ParentEntity = entity;
                        break;
                    case Tokens.DERIVE: //derives definitions
                        var derived = section.val as List<DerivedAttribute>;
                        if (derived != null && derived.Any())
                            foreach (var attribute in derived)
                                attribute.ParentEntity = entity;
                        break;
                    case Tokens.ABSTRACT: //inheritance definition
                        var isAbstract = section.boolVal;
                        if (isAbstract)
                            entity.Instantiable = false;

                        var identifiers = section.val as List<string>;
                        if(identifiers == null || !identifiers.Any())
                        break;
                        ToDoActions.Add(() =>
                        {
                            foreach (var type in identifiers.Select(typeName => Model.Get<EntityDefinition>(t => t.Name == typeName).FirstOrDefault()))
                            {
                                if(type == null)
                                    throw new InstanceNotFoundException();
                                if (entity.Supertypes == null) entity.Supertypes = new HashSet<EntityDefinition>();
                                entity.Supertypes.Add(type);
                            }
                        });
                        break;
                    case Tokens.WHERE:
                        var whereRules = section.val as List<WhereRule>;
                        if (whereRules != null && whereRules.Any())
                            foreach (var rule in whereRules)
                                rule.ParentItem = entity;
                        break;
                        
                    case Tokens.SELF: //Parameter section
                        var attributes = section.val as List<ExplicitAttribute>;
                        if (attributes != null && attributes.Any())
                            foreach (var attribute in attributes)
                                attribute.ParentEntity = entity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private UniquenessRule CreateUniquenessRule(string name, IEnumerable<string> attributes)
        {
            var result = Model.New<UniquenessRule>(r => r.Label = name);
            ToDoPostActions.Add(() =>
            {
                result.Attributes = new List<Attribute>();
                foreach (var attribute in attributes)
                {
                    var entity = result.ParentEntity;
                    if(entity == null)
                        throw new Exception("UniquenessRule has to be defined for a specific entity.");
                    var attr = entity.AllAttributes.FirstOrDefault(a => a.Name == attribute);
                    if(attr == null)
                        throw new InstanceNotFoundException();
                    result.Attributes.Add(attr);
                }
            });
            return result;
        }

        private WhereRule CreateWhereRule(string name)
        {
            var description = String.Format("{0},{1}", Scanner.yylloc.StartLine, Scanner.yylloc.EndLine);
            var rule = Model.New<WhereRule>(r => { 
                r.Label = name;
                r.Description = description;
            });
            return rule;
        }

        private ArrayType CreateArrayType(BaseType type, int upperIndex)
        {
            return Model.New<ArrayType>(t => {t.ElementType = type; t.UpperIndex = upperIndex;});
        }

        private DerivedAttribute CreateDerivedAttribute(string name)
        {
            return Model.New<DerivedAttribute>(a => a.Name = name);
        }

        private DerivedAttribute CreateDerivedAttribute(IEnumerable<string> path)
        {
            var result = Model.New<DerivedAttribute>(a => a.Name = path.Last());
            ToDoActions.Add(() =>
            {
                var type = Model.Get<EntityDefinition>(d => d.Name == path.First()).FirstOrDefault();
                if(type == null) 
                    throw new InstanceNotFoundException();
                var attr = type.ExplicitAttributes.FirstOrDefault(a => a.Name == path.Last());
                if (attr == null)
                    throw new InstanceNotFoundException();
                result.Redeclaring = attr;
            });

            return result;
        }

        private InverseAttribute CreateInverseAtribute(string name, string type, string attribute)
        {
            var result = Model.New<InverseAttribute>(a =>
            {
                a.Name = name;
            });
            ToDoActions.Add(() =>
            {
                var domain = Model.Schema.Entities.FirstOrDefault(e => e.Name == type);
                if(domain == null)
                    throw new InstanceNotFoundException();
                var attr = domain.AllExplicitAttributes.FirstOrDefault(a => a.Name == attribute);
                if (attr == null)
                    throw new InstanceNotFoundException();
                result.Domain = domain;
                result.InvertedAttr = attr;
            });

            return result;
        }


        private void CreateType(string name, ValueType typeBase, List<WhereRule> whereRules)
        {
            var type = Model.New<DefinedType>(e => e.Name = name);
            switch (typeBase.tokVal)
            {
                    case Tokens.IDENTIFIER:
                    ToDoActions.Add(() =>
                    {
                        var domain = Model.Get<NamedType>(t => t.Name == typeBase.strVal).FirstOrDefault();
                        if(domain == null) 
                            throw new InstanceNotFoundException();
                        type.Domain = domain as UnderlyingType;
                    });
                    break;
                    case Tokens.TYPE:
                    type.Domain = typeBase.val as UnderlyingType;
                    if(type.Domain == null) 
                        throw new InstanceNotFoundException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(whereRules == null) return;

            foreach (var rule in whereRules)
            {
                rule.ParentItem = type;
            }
        }

        private ExplicitAttribute CreateSimpleAttribute(ValueType data)
        {
            var result = Model.New<ExplicitAttribute>();
            if (data.tokVal == Tokens.TYPE)
            {
                result.Domain = data.val as SimpleType;
            }
            if (data.tokVal == Tokens.IDENTIFIER)
            {
                ToDoActions.Add(() =>
                {
                    result.Domain = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault();
                    if(result.Domain == null) 
                        throw new InstanceNotFoundException();
                });
            }
            return result;
        }

        private ExplicitAttribute NameAttribute(ExplicitAttribute attribute, string name, bool optional)
        {
            attribute.Name = name;
            attribute.OptionalFlag = optional;
            attribute.Line = Scanner.yylloc.StartLine;
            return attribute;
        }

        private ExplicitAttribute CreateEnumerableAttribute(AggregationType aggregationType, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>();
            result.Domain = aggregationType;
            aggregationType.UniqueElements = unique;

            if (data.tokVal == Tokens.TYPE)
            {
                aggregationType.ElementType = data.val as BaseType;
            }
            if (data.tokVal == Tokens.IDENTIFIER)
            {
                ToDoActions.Add(() =>
                {
                    aggregationType.ElementType = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault();
                    if (data.strVal == "IfcBinary")
                    {
                        var definition =
                            aggregationType.SchemaModel.Get<EntityDefinition>(t => t.Attributes.Contains(result));
                    }
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;

        }

        private ExplicitAttribute CreateEnumerableOfEnumerableAttribute(AggregationType outerAggregation, AggregationType innerAggregation, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>();
            result.Domain = outerAggregation;
            outerAggregation.ElementType = innerAggregation;
            outerAggregation.UniqueElements = unique;

            if (data.tokVal == Tokens.TYPE)
            {
                innerAggregation.ElementType = data.val as SimpleType;
            }
            if (data.tokVal == Tokens.IDENTIFIER)
            {
                ToDoActions.Add(() =>
                {
                    innerAggregation.ElementType = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault();
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;

        }

        private ExplicitAttribute CreateEnumerableOfEnumerableAttribute(AggregationType superOuterAggregation, AggregationType outerAggregation, AggregationType innerAggregation, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>();
            result.Domain = superOuterAggregation;
            superOuterAggregation.UniqueElements = unique;
            superOuterAggregation.ElementType = outerAggregation;
            outerAggregation.ElementType = innerAggregation;

            if (data.tokVal == Tokens.TYPE)
            {
                innerAggregation.ElementType = data.val as SimpleType;
            }
            if (data.tokVal == Tokens.IDENTIFIER)
            {
                ToDoActions.Add(() =>
                {
                    innerAggregation.ElementType = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault();
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;

        }

        private void CreateGlobalRule(string name, IEnumerable<string> identifiers)
        {
            var rule = Model.New<GlobalRule>(r => r.Name = name);
            ToDoActions.Add(() =>
            {
                rule.Entities = new List<EntityDefinition>();
                foreach (var identifier in identifiers)
                {
                    var typeName = identifier;
                    var type = Model.Get<EntityDefinition>(e => e.Name == typeName).FirstOrDefault();
                    if (type == null)
                        throw new InstanceNotFoundException();
                    rule.Entities.Add(type);
                }
            });
        }

    }
}
