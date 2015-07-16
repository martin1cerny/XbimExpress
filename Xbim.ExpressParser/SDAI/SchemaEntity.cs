using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public interface ISchemaEntity
    {
        SchemaModel SchemaModel { get; }
    }

    public abstract class SchemaEntity : ISchemaEntity
    {
        public SchemaModel SchemaModel { get; internal set; }
    }
}
