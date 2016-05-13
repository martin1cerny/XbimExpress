using System;
using System.Collections.Generic;
using System.Linq;
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
            while (true)
            {
                var expl = attribute as ExplicitAttribute;
                if (expl != null)
                    return TypeHelper.GetInterfaceCSType(expl, Settings);
                var der = attribute as DerivedAttribute;
                if (der == null) return "";

                attribute = der.Redeclaring;
            }
        }

        public string InterfaceNamespace
        {
            get { return Settings.Namespace + "." + Settings.SchemaInterfacesNamespace; }
        }

        public override string Inheritance
        {
            get
            {
                var items = base.Inheritance.Trim(' ', ':').Split(new []{ ',' } , StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToList();
                
                //add own interface
                items.Add("I" + Name);

                if (Type.Instantiable && AllExplicitAttributes.Any(IsEntityRefOrAggr))
                {
                    items.Add("IContainsEntityReferences");

                    var indexedAttributes = AllExplicitAttributes.Where(IsPartOfInverse).ToList();
                    if (indexedAttributes.Any())
                        items.Add("IContainsIndexedReferences");
                }
                
                

                //remove selects
                var selects = Type.IsInSelects.Select(s => s.Name.ToString());
                foreach (var @select in selects)
                {
                    items.Remove(@select);
                }

                //merge to a single string
                var i = string.Join(", ", items);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
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
                result.Add(InterfaceNamespace);
                result.Add(Namespace);

                return result.Distinct();
            }
        }

        private static bool IsEntityReference(BaseType type)
        {
            if (type is EntityDefinition)
                return true;

            var select = type as SelectType;
            if (select == null)
                return false;

            var realTypes = SelectTypeTemplate.GetFinalTypes(select);
            return realTypes.All(t => t is EntityDefinition);
        }

        private bool IsEntityRefOrAggr(ExplicitAttribute attribute)
        {
            if (IsEntityReference(attribute))
                return true;
            var aggr = attribute.Domain as AggregationType;
            if (aggr == null)
                return false;
            var nt = GetNamedElementType(aggr);
            return nt is EntityDefinition;
        }

        private static bool IsEntityReference(ExplicitAttribute attribute)
        {
            return IsEntityReference(attribute.Domain);
        }

        private static bool IsEntityReferenceAggregation(BaseType type)
        {
            var aggr = type as AggregationType;
            return aggr != null && IsEntityReference(aggr.ElementType);
        }

        private static bool IsEntityReferenceAggregation(ExplicitAttribute attribute)
        {
            return IsEntityReferenceAggregation(attribute.Domain);
        }

        private static bool IsEntityReferenceDoubleAggregation(BaseType type)
        {
            var aggr = type as AggregationType;
            return aggr != null && IsEntityReferenceAggregation(aggr.ElementType);
        }

        private static bool IsEntityReferenceDoubleAggregation(ExplicitAttribute attribute)
        {
            return IsEntityReferenceDoubleAggregation(attribute.Domain);
        }
    }
}
