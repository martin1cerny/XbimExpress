using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Templates
{
    public partial class SelectTypeTemplate : ICodeTemplate
    {
        public SelectType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;
        
        public SelectTypeTemplate(GeneratorSettings settings, SelectType type)
        {
            _settings = settings;
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

        public string InterfaceNamespace
        {
            get { return _settings.Namespace + "." + _settings.SchemaInterfacesNamespace; }
        }

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name.ToString()).ToList();
                if (parents.Count == 0) parents.Add("IExpressSelectType");

                //mark it as an entity if all subtypes are entities
                if (GetFinalTypes(Type).All(s => s is EntityDefinition))
                    parents.Add(_settings.PersistEntityInterface);

                //mark it as an espress type if all subtypes are defined types
                if (GetFinalTypes(Type).All(s => s is DefinedType))
                    parents.Add("IExpressValueType");

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
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        private static IEnumerable<NamedType> GetFinalTypes(SelectType select)
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
                    var helper = new NamedTypeHelper(type, _settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                result.Add(_settings.InfrastructureNamespace);
                result.Add(InterfaceNamespace);

                return result;
            }
        }
    }
}
