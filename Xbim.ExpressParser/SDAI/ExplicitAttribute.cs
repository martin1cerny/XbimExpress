using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class ExplicitAttribute:Attribute, ExplicitOrDerived
    {
        public BaseType Domain { get; set; }
        public ExplicitAttribute Redeclaring { get; set; }
        public bool OptionalFlag { get; set; }

        /// <summary>
        /// This is only important for serialization/deserialization/late binding
        /// </summary>
        public int Order { get; set; }
    }
}
