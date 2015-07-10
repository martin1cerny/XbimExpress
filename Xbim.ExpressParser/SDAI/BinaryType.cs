using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class BinaryType : SimpleType
    {
        public Boolean FixedWidth { get; set; }
        public int Width { get; set; }
    }
}
