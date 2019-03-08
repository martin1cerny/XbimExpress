using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates
{
    public partial class SelectTypeTemplate : ICodeTemplate
    {
        public SelectType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings Settings;
        
        public SelectTypeTemplate(GeneratorSettings settings, SelectType type)
        {
            Settings = settings;
            _helper = new NamedTypeHelper(type, settings);
            Type = type;
        }

        public string Namespace
        {
            get
            {
                return _helper.FullNamespace;
            }
        }

        public string InterfaceNamespace => Settings.SchemaInterfacesNamespace;

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name).ToList();
                if (Settings.GenerateInterfaces)
                    parents.Add("I" + Name);
                else
                {
                    if (parents.Count == 0) parents.Add("IExpressSelectType");

                    //mark it as an entity if all subtypes are entities
                    if (GetFinalTypes(Type).All(s => s is EntityDefinition))
                        parents.Add(Settings.PersistEntityInterface);


                    //mark it as an espress value type if all subtypes are defined types
                    if (GetFinalTypes(Type).All(s => s is DefinedType))
                        parents.Add("IExpressValueType");
                }
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        private string InterfaceInheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => "I" + s.Name).ToList();
                if (parents.Count == 0) parents.Add("IExpressSelectType");

                //mark it as an entity if all subtypes are entities
                if (GetFinalTypes(Type).All(s => s is EntityDefinition))
                    parents.Add(Settings.PersistEntityInterface);


                //mark it as an espress value type if all subtypes are defined types
                if (GetFinalTypes(Type).All(s => s is DefinedType))
                    parents.Add("IExpressValueType");

                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        public static IEnumerable<NamedType> GetFinalTypes(SelectType select)
        {
            foreach (var namedType in select.Selections)
            {
                var nested = namedType as SelectType;
                if (nested != null)
                    foreach (var type in GetFinalTypes(nested))
                    {
                        yield return type;
                    }
                else
                    yield return namedType;
            }
        }

        public IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();

                var selects = Type.IsInSelects.ToList();

                namedOccurances.AddRange(selects);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, Settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                result.Add(Settings.InfrastructureNamespace);
                if (Settings.GenerateInterfaces)
                    result.Add(InterfaceNamespace);

                return result;
            }
        }
    }
}
