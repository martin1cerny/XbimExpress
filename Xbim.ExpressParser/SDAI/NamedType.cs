using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class NamedType : SchemaEntity, BaseType, TypeOrRule
    {
        /// <summary>
        /// It might be necessary to change names of the types for a code generation for example to avoid name clashes
        /// but original persistance name from schema needs to be retained for serialization/deserialization. 
        /// This is an extension to SDAI schema.
        /// </summary>
        public string PersistanceName { get; set; }

        /// <summary>
        /// Id of the type is its order in EXPRESS schema file by default but it can be changed by user.
        /// This might be used to codify type as an int for databases, indexing etc. This is an extension to SDAI schema.
        /// </summary>
        public int TypeId { get; set; }

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
