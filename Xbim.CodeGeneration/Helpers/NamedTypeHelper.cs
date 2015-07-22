using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Helpers
{
    public class NamedTypeHelper
    {
        public NamedType Type { get; private set; }
        public DomainStructure Structure { get; private set; }

        private readonly GeneratorSettings _settings;

        public NamedTypeHelper(NamedType type, GeneratorSettings settings)
        {
            Type = type;
            Structure = settings.Structure;
            _settings = settings;
        }

        public string FullNamespace
        {
            get
            {
                var ns = _settings.Namespace;
                if (Structure == null) return ns;
                var domain = Structure.GetDomainForType(Type.Name);
                if (domain != null)
                    ns += "." + domain.Name;
                return ns;
            }
        }
    }
}
