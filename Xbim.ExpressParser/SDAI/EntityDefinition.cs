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
            get { return SchemaModel.Get<ExplicitAttribute>(e => e.ParentEntity == this); }
        }

        /// <summary>
        /// Explicit attributes including all inherited explicit attributes
        /// </summary>
        public IEnumerable<ExplicitAttribute> AllExplicitAttributes
        {
            get
            {
                //enumerate parent attributes first
                if(Supertypes != null)
                    foreach (var attribute in Supertypes.SelectMany(supertype => supertype.AllExplicitAttributes))
                    {
                        yield return attribute;
                    }
                foreach (var attribute in ExplicitAttributes)
                {
                    yield return attribute;
                }
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