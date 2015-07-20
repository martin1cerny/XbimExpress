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
    public partial class DefinedTypeTemplate
    {
        public DefinedType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;
        
        public DefinedTypeTemplate(GeneratorSettings settings, DefinedType type)
        {
            _settings = settings;
            _helper = new NamedTypeHelper(type, settings.Structure);
            Type = type;
        }

        public string Namespace
        {
            get
            {
                var ns = _helper.FullNamespace;
                var bns = _settings.Namespace;
                return String.Format("{0}.{1}", bns, ns);
            }
        }

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name.ToString()).ToList();
                parents.Insert(0, _settings.TypeSettings.BaseType);
                return String.Join(", ", parents);
            }
        }
    }
}
