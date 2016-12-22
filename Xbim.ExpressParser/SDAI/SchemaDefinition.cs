using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class SchemaDefinition : SchemaEntity
    {
        public string Source { get; set; }
        public string Name { get; set; }
        public string Identification { get; set; }

        #region Inverse members

        public IEnumerable<EntityDefinition> Entities
        {
            get { return SchemaModel.Get<EntityDefinition>(def => def.ParentSchema == this); }
        }

        public IEnumerable<DefinedType> Types
        {
            get { return SchemaModel.Get<DefinedType>(def => def.ParentSchema == this); }
        }

        public IEnumerable<EnumerationType> Enumerations
        {
            get { return SchemaModel.Get<EnumerationType>(def => def.ParentSchema == this); }
        }

        public IEnumerable<SelectType> SelectTypes
        {
            get { return SchemaModel.Get<SelectType>(def => def.ParentSchema == this); }
        }

        public IEnumerable<GlobalRule> GlobalRules
        {
            get { return SchemaModel.Get<GlobalRule>(r => r.ParentSchema == this); }
        }

        #endregion
    }
}