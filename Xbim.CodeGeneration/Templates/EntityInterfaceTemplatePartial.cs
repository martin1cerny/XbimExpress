﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityInterfaceTemplate
    {
        public EntityInterfaceTemplate(GeneratorSettings settings, EntityDefinition type): base(settings, type)
        {
            
        }

        // ReSharper disable once InconsistentNaming
        protected string GetInterfaceCSType(ExplicitOrDerived attribute)
        {
            var result = GetCSType(attribute);
            result = result.Replace("Optional" + Settings.ItemSetClassName, "IEnumerable");
            return result.Replace(Settings.ItemSetClassName, "IEnumerable");
        }

        public string InterfaceNamespace
        {
            get { return Settings.Namespace; }
        }

        public override string Inheritance
        {
            get
            {
                var i = base.Inheritance;
                if (string.IsNullOrWhiteSpace(i))
                    i = ": ";
                else
                {
                    i += ", ";
                }
                return i + "I" + Name;
            }
        }

        protected string InterfaceInheritance
        {
            get
            {
                var parents = new List<string>();
                if (IsFirst)
                    parents.Add(Settings.PersistEntityInterface);
                else
                    parents.AddRange(Type.Supertypes.Select(t => "I" + t.Name.ToString()));

                //add any select interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //merge to a single string
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            } 
        }

        public override IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = base.Using.ToList();
                result.Add(Namespace);

                return result;
            }
        }
    }
}
