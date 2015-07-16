using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class SchemaComparisonResult : ComparisonResult<SchemaDefinition>
    {
        public SchemaComparisonResult(SchemaDefinition oldDefinition, SchemaDefinition newDefinition)
            : base(oldDefinition, newDefinition)
        {
        }

        public override string OldObjectName
        {
            get { return OldObject != null ? OldObject.Name : null; }
        }

        public override string NewObjectName
        {
            get { return NewObject != null ? NewObject.Name : null; }
        }
    }
}