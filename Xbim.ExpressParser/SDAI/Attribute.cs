using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class Attribute: SchemaEntity
    {
        public ExpressId PersistentName { get; internal set; }

        //this is a helper to resolve order of attributes within the entity
        internal int Line { get; set; }

        public ExpressId Name
        {
            get; 
            set;
        }
        public EntityDefinition ParentEntity { get; set; }
    }
}
