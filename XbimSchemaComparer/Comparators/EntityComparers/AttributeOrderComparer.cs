using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.EntityComparers
{
    public class AttributeOrderComparer: ISchemaComparer<EntityDefinition, EntityAttributeComparisonResult>
    {
        private readonly List<EntityAttributeComparisonResult> _results = new List<EntityAttributeComparisonResult>(); 

        public Guid Id
        {
            get { return new Guid("B545DB33-40BC-433D-B82B-BE1122E0A4E6"); }
        }

        public IEnumerable<EntityAttributeComparisonResult> Compare(EntityDefinition oldObject, EntityDefinition newObject)
        {
            var results = new List<EntityAttributeComparisonResult>();

            var oldAttributes = oldObject.AllExplicitAttributes.ToList();
            var newAttributes = newObject.AllExplicitAttributes.ToList();

            var addedAttributes = newAttributes.Where(na => oldAttributes.All(oa => oa.Name != na.Name));
            var removedAttributes = oldAttributes.Where(na => newAttributes.All(oa => oa.Name != na.Name));
            var sameNameAttributes = newAttributes.Where(na => oldAttributes.Any(oa => oa.Name == na.Name));

            var result = new EntityAttributeComparisonResult(oldObject, newObject);
            foreach (var addedAttribute in addedAttributes)
            {
                result.Differences.Add(new AttributeDifference
                {
                    ResultType = ComparisonResultType.Added,
                    Attribute = addedAttribute, 
                    NewIndex = newAttributes.IndexOf(addedAttribute),
                    OldIndex = -1
                });
            }
            foreach (var removedAttribute in removedAttributes)
            {
                result.Differences.Add(new AttributeDifference
                {
                    ResultType = ComparisonResultType.Removed,
                    Attribute = removedAttribute,
                    OldIndex = oldAttributes.IndexOf(removedAttribute),
                    NewIndex = -1
                });
            }

            foreach (var newAttribute in sameNameAttributes)
            {
                var oldAttribute = oldAttributes.FirstOrDefault(a => a.Name == newAttribute.Name);
                if(oldAttribute == null) throw new Exception();

                var newIndex = newAttributes.IndexOf(newAttribute);
                var oldIndex = oldAttributes.IndexOf(oldAttribute);
                if(newIndex == oldIndex && IsSameDomain(oldAttribute.Domain, newAttribute.Domain)) continue;
                result.Differences.Add(new AttributeDifference
                {
                    ResultType = ComparisonResultType.Changed,
                    Attribute = newAttribute,
                    NewIndex = newIndex,
                    OldIndex = oldIndex,
                    NewDomain = newAttribute.Domain,
                    OldDomain = oldAttribute.Domain
                });
            }

            if (result.NumberOfDifferences <= 0) return results;

            results.Add(result);
            _results.Add(result);
            return results;
        }

        private bool IsSameDomain(BaseType o, BaseType n)
        {
            if (o.GetType() != n.GetType()) return false;

            var oNamed = o as NamedType;
            var nNamed = n as NamedType;

            if (oNamed != null && nNamed != null)
                return oNamed.Name == nNamed.Name;

            var oAggregate = o as AggregationType;
            var nAggregate = n as AggregationType;
            if (oAggregate != null && nAggregate != null)
                return IsSameDomain(oAggregate.ElementType, nAggregate.ElementType);

            return true;
        }

        public IEnumerable<EntityAttributeComparisonResult> ComparisonResults
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
            get { return "Attributes changed in entity"; }
        }
    }
}
