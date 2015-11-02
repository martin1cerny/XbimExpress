﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Differences
{
    public class ExplicitAttributeMatch
    {
        private readonly EntityDefinition _source;
        private readonly EntityDefinition _target;

        public bool IsTypeCompatible { get; private set; }

        private ExplicitAttributeMatch(EntityDefinition source, EntityDefinition target)
        {
            _source = source;
            _target = target;
        }

        public static IEnumerable<ExplicitAttributeMatch> FindMatches(EntityDefinition source, EntityDefinition target)
        {
            var allTarget = target.ExplicitAttributes.ToList();
            var allSource = source.AllExplicitAttributes.ToList();
            var onlySource = source.ExplicitAttributes.ToList();


            var identityCandidates = allSource.Where(
                        sa => allTarget.Any(ta => string.Compare(sa.Name, ta.Name, StringComparison.InvariantCultureIgnoreCase) == 0)).ToList();
            var removedCandidates = onlySource.Where(
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
                yield return new ExplicitAttributeMatch(source, target)
                {
                    //check type
                    MatchType = AttributeMatchType.Identity,
                    SourceAttribute = attribute,
                    TargetAttribute = nameMatch,
                    IsTypeCompatible = IsCompatibleSystemType(attribute.Domain, nameMatch.Domain)
                };
            }

            //if there is one attribute added and one replaced and they are of compatible type it should be a match
            if (removedCandidates.Count == 1 && addedCandidates.Count == 1)
            {
                var rc = removedCandidates[0];
                var ac = addedCandidates[0];
                if (IsCompatibleSystemType(rc.Domain, ac.Domain))
                {
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = rc,
                        TargetAttribute = ac,
                        IsTypeCompatible = true
                    };
                    yield break;
                }
            }

            //these are potentially removed unless we find an unique match based on other criteria
            foreach (var attribute in addedCandidates)
            {
                //try to find match by type name
                var candidates = removedCandidates.Where(ac => IsTypeNameMatch(ac.Domain, attribute.Domain)).ToList();
                if (candidates.Count == 1)
                {
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0],
                        IsTypeCompatible = true
                    };
                    continue;
                }

                //try to find match by compatible type
                candidates = removedCandidates.Where(ac => IsCompatibleSystemType(ac.Domain, attribute.Domain)).ToList();
                if (candidates.Count == 1)
                {
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0],
                        IsTypeCompatible = true
                    };
                    continue;
                }

                //try to find a match by minor difference in the name (like added 's' at the end or similar)
                candidates = removedCandidates.Where(ac => ac.Name.ToString().ToUpperInvariant().LevenshteinDistance(attribute.Name.ToString().ToUpperInvariant()) < 4).ToList();
                if (candidates.Count == 1 && IsCompatibleSystemType(attribute.Domain, candidates[0].Domain))
                {
                    yield return new ExplicitAttributeMatch(source, target)
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0],
                        IsTypeCompatible = true
                    };
                    continue;
                }
                
                yield return new ExplicitAttributeMatch(source, target)
                {
                    MatchType = AttributeMatchType.NotFound,
                    SourceAttribute = attribute,
                    IsTypeCompatible = false,
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

        private static bool IsCompatibleSystemType(BaseType o, BaseType n)
        {
            while (true)
            {
                if (o.GetType() != n.GetType()) return false;

                var oE = o as EntityDefinition;
                var nE = n as EntityDefinition;
                if (oE != null && nE != null)
                {
                    return IsEntityMatch(oE, nE) || IsSubEntityMatch(oE, nE);
                }

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

        private static bool IsEntityMatch(NamedType o, NamedType n)
        {
            return string.Compare(o.Name, n.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private static bool IsSuperEntityMatch(EntityDefinition o, EntityDefinition n)
        {
            if (o.Supertypes == null || !o.Supertypes.Any()) return false;
            var supertype = o.Supertypes.First();
            while (supertype != null)
            {
                if (IsEntityMatch(supertype, n)) return true;
                if (supertype.Supertypes == null || !supertype.Supertypes.Any()) return false;
                supertype = supertype.Supertypes.First();
            }
            return false;
        }

        private static bool IsSubEntityMatch(EntityDefinition o, EntityDefinition n)
        {
            var subtypes = n.AllSubTypes;
            return subtypes.Any(s => IsEntityMatch(s, o));
        }

        public ExplicitAttribute SourceAttribute { get; private set; }
        //public int SourceAttributeOrder {
        //    get { return _source.AllExplicitAttributes.ToList().IndexOf(SourceAttribute); }
        //}
        public ExplicitAttribute TargetAttribute { get; private set; }
        //public int TargetAttributeOrder
        //{
        //    get { return _target.AllExplicitAttributes.ToList().IndexOf(TargetAttribute); }
        //}
        public AttributeMatchType MatchType { get; private set; }

        //public bool ChangedOrder { get { return SourceAttributeOrder != TargetAttributeOrder; } }

        public string Message { get; private set; }
    }

    public enum AttributeMatchType
    {
        Identity,
        Changed,
        NotFound
    }
}
