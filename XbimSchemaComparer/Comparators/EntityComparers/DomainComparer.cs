using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.EntityComparers
{
    public class DomainComparer : ISchemaComparer<EntityDefinition, EntityDomainComparisonResult>
    {
        private readonly List<EntityDomainComparisonResult> _results = new List<EntityDomainComparisonResult>(); 
        public Guid Id
        {
            get { return new Guid("35F75DC8-4AB4-4C52-A77E-C444DC5708C8"); }
        }

        private DomainStructure _oldDomains;
        private DomainStructure _newDomains;

        public IEnumerable<EntityDomainComparisonResult> Compare(EntityDefinition oldObject, EntityDefinition newObject)
        {
            var results = new List<EntityDomainComparisonResult>();
            if (_oldDomains == null)
                _oldDomains = GetDomain(oldObject.SchemaModel.FirstSchema);
            if (_newDomains == null)
                _newDomains = GetDomain(newObject.SchemaModel.FirstSchema);
            if (_oldDomains == null || _newDomains == null)
                return results;

            var oldDomain = _oldDomains.GetDomainForType(oldObject.Name);
            var newDomain = _newDomains.GetDomainForType(newObject.Name);

            if (oldDomain.Name.ToLower() == newDomain.Name.ToLower()) return results;

            var result = new EntityDomainComparisonResult(oldObject, newObject)
            {
                OldDomain = oldDomain.Name,
                NewDomain = newDomain.Name
            };
            results.Add(result);
            _results.Add(result);

            return results;
        }

        private static DomainStructure GetDomain(SchemaDefinition schema)
        {
            switch (schema.Source)
            {
                case SchemaSources.IFC2x3_TC1:
                    return DomainStructure.LoadIfc2X3();
                case SchemaSources.IFC4:
                    return DomainStructure.LoadIfc4();
                case SchemaSources.IFC4_ADD1:
                    return DomainStructure.LoadIfc4Add1();
                case SchemaSources.IFC4_ADD2:
                    return DomainStructure.LoadIfc4Add2();
                default:
                    return null;
            }
        }

        public IEnumerable<EntityDomainComparisonResult> ComparisonResults
        {
            get { return _results; }
        }

        public Type ResultType
        {
            get { return typeof(EntityDefinition); }
        }

        public IEnumerable<IComparisonResult> Compare(object oldObject, object newObject)
        {
            return Compare(oldObject as EntityDefinition, newObject as EntityDefinition);
        }

        public IEnumerable<IComparisonResult> Results
        {
            get { return _results; }
        }

        public string Name
        {
            get { return "Domain comparison"; }
        }
    }
}
