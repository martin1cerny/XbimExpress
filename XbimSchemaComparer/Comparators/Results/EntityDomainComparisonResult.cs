using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class EntityDomainComparisonResult : EntityComparisonResult
    {
        public EntityDomainComparisonResult(EntityDefinition o, EntityDefinition n): base (o,n)
        {
            
        }

        public string OldDomain { get; set; }
        public string NewDomain { get; set; }

        public override string Message
        {
            get { return String.Format("{0}: Old domain: {1}, New domain: {2}", NewObject.Name, OldDomain, NewDomain); }
            set {  }
        }
    }
}
