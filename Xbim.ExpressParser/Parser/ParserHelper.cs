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

        private List<Action> ToDoActions = new List<Action>();

        //do all postprocessing, assignments and other operations
        private void Finish()
        {
            foreach (var action in ToDoActions)
            {
                action();
            }

            //clear for the next processing/parsing
            ToDoActions.Clear();
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

        private void CreateEntity(string name, List<ValueType> sections)
        {
            var entity = Model.New<EntityDefinition>(e => e.Name = name);
            foreach (var section in sections)
            {
                switch (section.tokVal)
                {
                    case Tokens.UNIQUE:
                        break;
                    case Tokens.INVERSE:
                        break;
                    case Tokens.DERIVE:
                        break;
                    case Tokens.ABSTRACT:
                        break;
                    case Tokens.WHERE:
                        break;
                        
                    case Tokens.SELF: //Parameter section
                        var attributes = section.val as List<ExplicitAttribute>;
                        if (attributes != null && attributes.Any())
                        {
                            foreach (var attribute in attributes)
                            {
                                attribute.ParentEntity = entity;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void CreateType(string name)
        {
            Model.New<DefinedType>(e => e.Name = name);
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
            return attribute;
        }

    }
}
