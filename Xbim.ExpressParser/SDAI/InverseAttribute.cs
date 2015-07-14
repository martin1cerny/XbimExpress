using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class InverseAttribute: Attribute
    {
        public EntityDefinition Domain { get; set; }
        public InverseAttribute Redeclaring { get; set; }
        public ExplicitAttribute InvertedAttr { get; set; }
        public bool Duplicates { get; set; }
    }
}
