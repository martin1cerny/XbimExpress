using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimTranslatorGenerator.Differences
{
    public class ExplicitAttributeMatch
    {
        private readonly EntityDefinition _source;
        private readonly EntityDefinition _target;

        private ExplicitAttributeMatch(EntityDefinition source, EntityDefinition target)
        {
            _source = source;
            _target = target;
        }

        public static IEnumerable<ExplicitAttributeMatch> FindMatches(EntityDefinition source, EntityDefinition target)
        {
            var allTarget = target.AllExplicitAttributes.ToList();
            var allSource = source.AllExplicitAttributes.ToList();

            var identityCandidates = allSource.Where(
                        sa => allTarget.Any(ta => string.Compare(sa.Name, ta.Name, StringComparison.InvariantCultureIgnoreCase) == 0)).ToList();
            var removedCandidates = allSource.Where(
                        sa => allTarget.All(ta => string.Compare(sa.Name, ta.Name, StringComparison.InvariantCultureIgnoreCase) != 0)).ToList();
            var addedCandidates = allTarget.Where(
                        sa => allSource.All(ta => string.Compare(sa.Name, ta.Name, StringComparison.InvariantCultureIgnoreCase) != 0)).ToList();

            foreach (var attribute in identityCandidates)
            {
                //try to find exact match 
                var nameMatch =
                    allTarget.FirstOrDefault(
                        ta => string.Compare(attribute.Name, ta.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (nameMatch == null) continue;
                //check type
                if(IsParserCompatibleType(attribute.Domain, nameMatch.Domain))
                {
                    var result = new ExplicitAttributeMatch(source, target)
                    {
                        SourceAttribute = attribute, 
                        TargetAttribute = nameMatch
                    };
                    result.MatchType = result.ChangedOrder ? AttributeMatchType.Changed : AttributeMatchType.Identity;
                    yield return result;
                    continue;
                }
                yield return new ExplicitAttributeMatch(source, target)
                {
                    MatchType = AttributeMatchType.NotFound,
                    SourceAttribute = attribute,
                    Message = "Attribute with the same name exists but has an incompatible type."
                };
            }

            //if there is one attribute added and one replaced and they are of compatible type it should be a match
            if (removedCandidates.Count == 1 && addedCandidates.Count == 1)
            {
                var rc = removedCandidates[0];
                var ac = addedCandidates[0];
                if (IsParserCompatibleType(rc.Domain, ac.Domain))
                {
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = rc,
                        TargetAttribute = ac
                    };
                    yield break;
                }
            }

            //these are potentially removed unless we find a match based on other criteria
            foreach (var attribute in removedCandidates)
            {
                //try to find match by type name
                var candidates = addedCandidates.Where(ac => IsTypeNameMatch(ac.Domain, attribute.Domain)).ToList();
                if (candidates.Count == 1)
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0]
                    };

                //try to find match by compatible type
                candidates = addedCandidates.Where(ac => IsParserCompatibleType(ac.Domain, attribute.Domain)).ToList();
                if (candidates.Count == 1)
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0]
                    };

                //try to find a match by minor difference in the name (like added 's' at the end or similar)
                candidates = addedCandidates.Where(ac => ac.Name.ToString().ToUpperInvariant().LevenshteinDistance(attribute.Name.ToString().ToUpperInvariant()) < 4).ToList();
                if (candidates.Count == 1 && IsParserCompatibleType(attribute.Domain, candidates[0].Domain))
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0]
                    };
                
                yield return new ExplicitAttributeMatch(source, target)
                {
                    MatchType = AttributeMatchType.NotFound,
                    SourceAttribute = attribute,
                    Message = "Matching attribute not found."
                };
            }
        }

        private static bool IsTypeNameMatch(BaseType o, BaseType n)
        {
            while (true)
            {
                if (o.GetType() != n.GetType()) return false;

                var oSimple = o as SimpleType;
                var nSimple = n as SimpleType;
                if (oSimple != null && nSimple != null)
                    return oSimple.GetType() == nSimple.GetType();

                //it might also be generally identical named type
                var oNamed = o as NamedType;
                var nNamed = n as NamedType;
                if (oNamed != null && nNamed != null)
                    return string.Compare(oNamed.Name, nNamed.Name, StringComparison.InvariantCultureIgnoreCase) == 0;

                var oAggregate = o as AggregationType;
                var nAggregate = n as AggregationType;
                if (oAggregate == null || nAggregate == null) return true;
                o = oAggregate.ElementType;
                n = nAggregate.ElementType;
            }
        }

        private static bool IsParserCompatibleType(BaseType o, BaseType n)
        {
            while (true)
            {
                if (o.GetType() != n.GetType()) return false;

                //if it is both entity definition lets suppose it will be compatible in parser
                if (o is EntityDefinition && n is EntityDefinition)
                    return true;

                var oSimple = o as SimpleType;
                var nSimple = n as SimpleType;
                if (oSimple != null && nSimple != null)
                    return oSimple.GetType() == nSimple.GetType();

                //it is enough if it is a compatible defined type
                var oDefined = o as DefinedType;
                var nDefined = n as DefinedType;
                if (oDefined != null && nDefined != null)
                    //check base type
                    if (oDefined.Domain.GetType() == nDefined.Domain.GetType())
                        return true;

                //it might also be generally identical named type
                var oNamed = o as NamedType;
                var nNamed = n as NamedType;
                if (oNamed != null && nNamed != null)
                    return string.Compare(oNamed.Name, nNamed.Name, StringComparison.InvariantCultureIgnoreCase) == 0;

                var oAggregate = o as AggregationType;
                var nAggregate = n as AggregationType;
                if (oAggregate == null || nAggregate == null) return true;
                o = oAggregate.ElementType;
                n = nAggregate.ElementType;
            }
        }

        public ExplicitAttribute SourceAttribute { get; private set; }
        public int SourceAttributeOrder {
            get { return _source.AllExplicitAttributes.ToList().IndexOf(SourceAttribute); }
        }
        public ExplicitAttribute TargetAttribute { get; private set; }
        public int TargetAttributeOrder
        {
            get { return _target.AllExplicitAttributes.ToList().IndexOf(TargetAttribute); }
        }
        public AttributeMatchType MatchType { get; private set; }

        public bool ChangedOrder { get { return SourceAttributeOrder != TargetAttributeOrder; } }

        public string Message { get; private set; }
    }

    public enum AttributeMatchType
    {
        Identity,
        Changed,
        NotFound
    }
}
