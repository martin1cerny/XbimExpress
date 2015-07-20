using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Helpers
{
    public class NamedTypeHelper
    {
        public NamedType Type { get; private set; }
        public DomainStructure Structure { get; set; }
        
        public NamedTypeHelper(NamedType type, DomainStructure structure)
        {
            Type = type;
            Structure = structure;
        }

        public string FullNamespace
        {
            get
            {
                var ns = Type.ParentSchema.Name;
                if (Structure != null)
                {
                    var domain = Structure.GetDomainForType(Type.Name);
                    if (domain != null)
                        ns += "." + domain.Name;
                }
                if (String.IsNullOrWhiteSpace(ns)) ns = "Generated";
                return ns;
            }
        }
    }
}
