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

        private SchemaDefinition _currentSchema;

        private Dictionary<SchemaDefinition, Dictionary<string, string>> _aliases =
            new Dictionary<SchemaDefinition, Dictionary<string, string>>();

        private Dictionary<string, string> CurrentAliases
        {
            get { return _aliases[_currentSchema]; }
        }

        internal Parser(Scanner lex) : base(lex)
        {
            Model = new SchemaModel();
            _currentSchema = Model.FirstSchema;
            _aliases.Add(_currentSchema, new Dictionary<string, string>());
        }

        // ReSharper disable once InconsistentNaming
        private readonly List<Action> ToDoActions = new List<Action>();
        // ReSharper disable once InconsistentNaming
        private readonly List<Action> ToDoPostActions = new List<Action>();

        private void FinishSchema(string name)
        {
            _currentSchema.Name = name;
            _currentSchema.Identification = name;

            //create new current schema and aliases for it
            _currentSchema = Model.New<SchemaDefinition>(null);
            _aliases.Add(_currentSchema, new Dictionary<string, string>());
        }

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

            //remove last added schema which is empty
            Model.Remove(_currentSchema);
            _currentSchema = null;
        }

        private void AddAlias(string original, string map)
        {
            CurrentAliases.Add(map, original);
        }

        private T GetNamedAlias<T>(SchemaDefinition schema, string mapName) where T : NamedType
        {
            var dict = _aliases[schema];
            string name;
            return dict.TryGetValue(mapName, out name) ? Model.Get<T>(t => t.Name == name).FirstOrDefault() : null;
        }

        private void CreateEnumeration(string name, List<string> values)
        {
            Model.New(_currentSchema, (EnumerationType e) =>
            {
                e.Name = name;
                e.Elements = new List<ExpressId>();
                e.Elements.AddRange(values);
            });
        }

        private void CreateSelectType(string name, List<string> selections)
        {
            var select = Model.New(_currentSchema, (SelectType e) =>
            {
                e.Name = name;
                e.Selections = new List<NamedType>();
            });

            foreach (var selection in selections)
            {
                ToDoActions.Add(() =>
                {
                    var type = Model.Get<NamedType>(n => n.Name == selection).FirstOrDefault() ??
                               GetNamedAlias<NamedType>(@select.ParentSchema, selection);
                    if (type == null)
                        throw new InstanceNotFoundException();
                    select.Selections.Add(type);
                });
            }
        }

        private int _lastEntityId = 0;
        private void CreateEntity(string name, IEnumerable<ValueType> sections)
        {
            var entity = Model.New<EntityDefinition>(_currentSchema, e =>
            {
                e.Name = name;
                e.PersistanceName = name.ToUpperInvariant();
                //entities are instantiable by default
                e.Instantiable = true;
                e.TypeId = _lastEntityId++;
            });
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
                        entity.Instantiable = !isAbstract;

                        var identifiers = section.val as List<string>;
                        if (identifiers == null || !identifiers.Any())
                            break;
                        ToDoActions.Add(() =>
                        {
                            foreach (
                                var type in
                                    identifiers.Select(
                                        typeName =>
                                            Model.Get<EntityDefinition>(t => t.Name == typeName).FirstOrDefault() ??
                                            GetNamedAlias<EntityDefinition>(entity.ParentSchema, typeName)))
                            {
                                if (type == null)
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
            var result = Model.New<UniquenessRule>(_currentSchema, r => r.Label = name);
            ToDoPostActions.Add(() =>
            {
                result.Attributes = new List<Attribute>();
                foreach (var attribute in attributes)
                {
                    var entity = result.ParentEntity;
                    if (entity == null)
                        throw new Exception("UniquenessRule has to be defined for a specific entity.");
                    var attr = entity.AllAttributes.FirstOrDefault(a => a.Name == attribute);
                    if (attr == null)
                        throw new InstanceNotFoundException();
                    result.Attributes.Add(attr);
                }
            });
            return result;
        }

        private WhereRule CreateWhereRule(string name)
        {
            var description = String.Format("{0},{1}", Scanner.yylloc.StartLine, Scanner.yylloc.EndLine);
            var rule = Model.New<WhereRule>(_currentSchema, r =>
            {
                r.Label = name;
                r.Description = description;
            });
            return rule;
        }

        private ArrayType CreateArrayType(BaseType type, int upperIndex)
        {
            return Model.New<ArrayType>(_currentSchema, t =>
            {
                t.ElementType = type;
                t.UpperIndex = upperIndex;
            });
        }

        private DerivedAttribute CreateDerivedAttribute(string name)
        {
            return Model.New<DerivedAttribute>(_currentSchema, a => a.Name = name);
        }

        private DerivedAttribute CreateDerivedAttribute(IEnumerable<string> path)
        {
            var result = Model.New<DerivedAttribute>(_currentSchema, a => a.Name = path.Last());
            ToDoActions.Add(() =>
            {
                var type = Model.Get<EntityDefinition>(d => d.Name == path.First()).FirstOrDefault() ??
                                            GetNamedAlias<EntityDefinition>(result.ParentEntity.ParentSchema, path.First());
                if (type == null)
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
            var result = Model.New<InverseAttribute>(_currentSchema, a =>
            {
                a.Name = name;
                a.PersistentName = name;
            });
            ToDoActions.Add(() =>
            {
                var domain = Model.Get<EntityDefinition>().FirstOrDefault(e => e.Name == type) ??
                                            GetNamedAlias<EntityDefinition>(result.ParentEntity.ParentSchema, type);
                if (domain == null)
                    throw new InstanceNotFoundException();
                var attr = domain.AllExplicitAttributes.FirstOrDefault(a => a.Name == attribute);
                if (attr == null)
                    throw new InstanceNotFoundException();
                result.Domain = domain;
                result.InvertedAttr = attr;
            });

            return result;
        }


        private void CreateTypeEnumerable(string name, AggregationType aggregation, ValueType typeBase,
            List<WhereRule> whereRules)
        {
            var type = Model.New<DefinedType>(_currentSchema, e =>
            {
                e.Name = name;
                e.PersistanceName = name.ToUpperInvariant();
                e.Domain = aggregation;
            });

            switch (typeBase.tokVal)
            {
                case Tokens.IDENTIFIER:
                    ToDoActions.Add(() =>
                    {
                        var domain = Model.Get<NamedType>(t => t.Name == typeBase.strVal).FirstOrDefault() ??
                                            GetNamedAlias<NamedType>(type.ParentSchema, typeBase.strVal);
                        if (domain == null)
                            throw new InstanceNotFoundException();
                        aggregation.ElementType = domain;
                    });
                    break;
                case Tokens.TYPE:
                    aggregation.ElementType = typeBase.val as BaseType;
                    if (type.Domain == null)
                        throw new InstanceNotFoundException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (whereRules == null) return;

            foreach (var rule in whereRules)
            {
                rule.ParentItem = type;
            }
        }

        private void CreateType(string name, ValueType typeBase, List<WhereRule> whereRules)
        {
            var type = Model.New<DefinedType>(_currentSchema, e =>
            {
                e.Name = name;
                e.PersistanceName = name.ToUpperInvariant();
            });
            switch (typeBase.tokVal)
            {
                case Tokens.IDENTIFIER:
                    ToDoActions.Add(() =>
                    {
                        var domain = Model.Get<NamedType>(t => t.Name == typeBase.strVal).FirstOrDefault() ??
                                            GetNamedAlias<NamedType>(type.ParentSchema, typeBase.strVal);
                        if (domain == null)
                            throw new InstanceNotFoundException();
                        type.Domain = domain as UnderlyingType;
                    });
                    break;
                case Tokens.TYPE:
                    type.Domain = typeBase.val as UnderlyingType;
                    if (type.Domain == null)
                        throw new InstanceNotFoundException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (whereRules == null) return;

            foreach (var rule in whereRules)
            {
                rule.ParentItem = type;
            }
        }

        private ExplicitAttribute CreateSimpleAttribute(ValueType data)
        {
            var result = Model.New<ExplicitAttribute>(_currentSchema);
            if (data.tokVal == Tokens.TYPE)
            {
                result.Domain = data.val as SimpleType;
            }
            if (data.tokVal == Tokens.IDENTIFIER)
            {
                ToDoActions.Add(() =>
                {
                    result.Domain = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault() ??
                                            GetNamedAlias<NamedType>(result.ParentEntity.ParentSchema, data.strVal);
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;
        }

        private ExplicitAttribute RedefineAttribute(ExplicitAttribute attribute, IEnumerable<string> accessor,
            bool optional)
        {
            var attrName = accessor.LastOrDefault();
            if (attrName == null) throw new Exception("Name of redeclaring attribute not defined.");
            var attr = NameAttribute(attribute, attrName, optional);
            ToDoPostActions.Add(() =>
            {
                var entity = attribute.ParentEntity;
                var redeclaring = entity.AllExplicitAttributes.FirstOrDefault(a => a.Name == attrName);
                if (redeclaring == null) throw new InstanceNotFoundException();

                attr.Redeclaring = redeclaring;
            });
            return attr;
        }

        private ExplicitAttribute NameAttribute(ExplicitAttribute attribute, string name, bool optional)
        {
            attribute.Name = name;
            attribute.PersistentName = name;
            attribute.OptionalFlag = optional;
            attribute.Line = Scanner.yylloc.StartLine;
            return attribute;
        }

        private ExplicitAttribute CreateEnumerableAttribute(AggregationType aggregationType, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>(_currentSchema);
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

        private ExplicitAttribute CreateEnumerableOfEnumerableAttribute(AggregationType outerAggregation,
            AggregationType innerAggregation, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>(_currentSchema);
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
                    innerAggregation.ElementType = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault() ??
                                            GetNamedAlias<NamedType>(result.ParentEntity.ParentSchema, data.strVal);
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;
        }

        private ExplicitAttribute CreateEnumerableOfEnumerableAttribute(AggregationType superOuterAggregation,
            AggregationType outerAggregation, AggregationType innerAggregation, ValueType data, bool unique)
        {
            var result = Model.New<ExplicitAttribute>(_currentSchema);
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
                    innerAggregation.ElementType = Model.Get<NamedType>(t => t.Name == data.strVal).FirstOrDefault() ??
                                            GetNamedAlias<NamedType>(result.ParentEntity.ParentSchema, data.strVal);
                    if (result.Domain == null)
                        throw new InstanceNotFoundException();
                });
            }
            return result;
        }

        private void CreateGlobalRule(string name, IEnumerable<string> identifiers)
        {
            var rule = Model.New<GlobalRule>(_currentSchema, r => r.Name = name);
            ToDoActions.Add(() =>
            {
                rule.Entities = new List<EntityDefinition>();
                foreach (var identifier in identifiers)
                {
                    var typeName = identifier;
                    var type = Model.Get<EntityDefinition>(e => e.Name == typeName).FirstOrDefault() ??
                                            GetNamedAlias<EntityDefinition>(rule.ParentSchema, typeName);
                    if (type == null)
                        throw new InstanceNotFoundException();
                    rule.Entities.Add(type);
                }
            });
        }
    }
}