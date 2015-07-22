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

        private string UnderlyingType
        {
            get { return TypeHelper.GetCSType(Type.Domain); }
        }

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name.ToString()).ToList();
                //parents.Insert(0, _settings.TypeSettings.BaseType);
                var i = String.Join(", ", parents);
                if (String.IsNullOrWhiteSpace(i))
                    return "";
                return ": " + i;
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

                var namedDomain = Type.Domain as NamedType;
                if(namedDomain != null)
                    namedOccurances.Add(namedDomain);

                var aggregation = Type.Domain as AggregationType;
                if(aggregation != null)
                    result.Add("System.Collections.Generic");

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
