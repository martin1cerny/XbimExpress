using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Templates
{
    public partial class DefinedTypeTemplate : ICodeTemplate
    {
        public DefinedType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;
        
        public DefinedTypeTemplate(GeneratorSettings settings, DefinedType type)
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

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name.ToString()).ToList();
                //parents.Insert(0, _settings.TypeSettings.BaseType);
                return String.Join(", ", parents);
            }
        }


        public IEnumerable<string> Using
        {
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

                return result;
            }
        }
    }
}
