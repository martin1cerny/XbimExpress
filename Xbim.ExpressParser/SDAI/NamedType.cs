using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class NamedType : SchemaEntity, BaseType, TypeOrRule
    {
        public ExpressId Name { get; set; }
        public SchemaDefinition ParentSchema { get; set; }

        #region Inverse
        /// <summary>
        /// Where rules defined for this type. This is defined as an explicit attribute in SDAI schema but
        /// it breaks general concept and it is in clash with 'ParentItem' attribute of the WhereRule.
        /// </summary>
        public IEnumerable<WhereRule> WhereRules
        {
            get { return SchemaModel.Get<WhereRule>(r => r.ParentItem == this); }
        }

        public IEnumerable<SelectType> IsInSelects
        {
            get { return SchemaModel.Get<SelectType>(s => s.Selections != null && s.Selections.Contains(this)); }
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}
