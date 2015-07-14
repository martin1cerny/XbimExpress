using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

namespace Xbim.ExpressParser.SDAI
{
    public class EntityDefinition : NamedType
    {
        public HashSet<EntityDefinition> Supertypes { get; set; }
        public bool Complex { get; set; }
        public bool Instantiable { get; set; }
        public bool Independent { get; set; }

        #region Inverse attributes

        public IEnumerable<Attribute> Attributes
        {
            get { return SchemaModel.Get<Attribute>(e => e.ParentEntity == this); }
        }

        /// <summary>
        /// Explicit attributes of this entity
        /// </summary>
        public IEnumerable<ExplicitAttribute> ExplicitAttributes
        {
            get { return SchemaModel.Get<ExplicitAttribute>(e => e.ParentEntity == this).OrderBy(a => a.Order); }
        }

        /// <summary>
        /// Explicit attributes including all inherited explicit attributes
        /// </summary>
        public IEnumerable<ExplicitAttribute> AllExplicitAttributes
        {
            get
            {
                var all = ExplicitAttributes.ToList();
                if (Supertypes == null)
                    return all;

                foreach (var supertype in Supertypes)
                {
                    all.AddRange(supertype.AllExplicitAttributes);
                }
                return all.OrderBy(a => a.Order);
            }
        }

        #endregion

        public void AddAttribute(Attribute attribute)
        {
            attribute.ParentEntity = this;
            //make sure both live in the same model
            attribute.SchemaModel = SchemaModel;
        }
    }
}