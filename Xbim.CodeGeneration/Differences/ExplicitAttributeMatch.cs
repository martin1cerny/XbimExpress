using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Differences
{
    public class ExplicitAttributeMatch
    {

        private static readonly List<Tuple<string,string,string>> ChangedNames = new List<Tuple<string, string, string>>(); 
        public bool IsTypeCompatible { get; private set; }

        private ExplicitAttributeMatch()
        {
        }

        static ExplicitAttributeMatch()
        {
            var data = Properties.Resources.Ifc4_ChangedNameProperties;
            var reader = new StringReader(data);
            var line = reader.ReadLine();
            while (!string.IsNullOrWhiteSpace(line))
            {
                var parts = line.Split(',').Select(v => v.Trim()).ToList();
                ChangedNames.Add(new Tuple<string, string, string>(parts[0], parts[1], parts[2]));
                line = reader.ReadLine();
            }
        }

        private static string GetNewName(string type, string property)
        {
            var candidate = ChangedNames.FirstOrDefault(t =>
                string.Equals(type, t.Item1, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(property, t.Item2, StringComparison.InvariantCultureIgnoreCase)
                );
            return candidate?.Item3;
        }

        private static string GetOldName(string type, string property)
        {
            var candidate = ChangedNames.FirstOrDefault(t =>
                string.Equals(type, t.Item1, StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(property, t.Item3, StringComparison.InvariantCultureIgnoreCase)
                );
            return candidate?.Item2;
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
                yield return new ExplicitAttributeMatch
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
                    yield return new ExplicitAttributeMatch
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = rc,
                        TargetAttribute = ac,
                        IsTypeCompatible = true
                    };
                    yield break;
                }
            }

            //try name containment match
            var toRemove = new List<ExplicitAttribute>();
            foreach (var attribute in addedCandidates)
            {
                string aName = attribute.Name;
                var oldName = GetOldName(source.Name, aName);
                var candidates = oldName != null ?
                    allSource.Where(ac => ac.Name == oldName).ToList():
                    removedCandidates.Where(ac => ((string)ac.Name).Contains(aName) || aName.Contains(ac.Name)).ToList();
                if (candidates.Count == 1 && IsCompatibleSystemType(attribute.Domain, candidates[0].Domain))
                {
                    toRemove.Add(attribute);
                    yield return new ExplicitAttributeMatch
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = candidates[0],
                        TargetAttribute = attribute,
                        IsTypeCompatible = true
                    };
                }
            }
            foreach (var attribute in toRemove)
            {
                //remove identified attributes
                addedCandidates.Remove(attribute);
            }


            //these are potentially removed unless we find an unique match based on other criteria
            foreach (var attribute in addedCandidates)
            {
                //try to find match by type name
                var candidates = removedCandidates.Where(ac => IsTypeNameMatch(ac.Domain, attribute.Domain)).ToList();
                if (candidates.Count == 1)
                {
                    yield return new ExplicitAttributeMatch
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
                    yield return new ExplicitAttributeMatch
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
                    yield return new ExplicitAttributeMatch
                    {
                        MatchType = AttributeMatchType.Changed,
                        SourceAttribute = attribute,
                        TargetAttribute = candidates[0],
                        IsTypeCompatible = true
                    };
                    continue;
                }
                
                yield return new ExplicitAttributeMatch
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
                {
                    if (
                        (oNamed.Name == "IfcStructuralSurfaceTypeEnum" && nNamed.Name == "IfcStructuralSurfaceMemberTypeEnum") ||
                        (oNamed.Name == "IfcStructuralCurveTypeEnum" && nNamed.Name == "IfcStructuralCurveMemberTypeEnum")
                        )
                        return true;
                    return string.Compare(oNamed.Name, nNamed.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
                }

                var oAggregate = o as AggregationType;
                var nAggregate = n as AggregationType;
                if (oAggregate == null || nAggregate == null) return true;
                o = oAggregate.ElementType;
                n = nAggregate.ElementType;
            }
        }

        private static IEnumerable<NamedType> GetAllSpecific(SelectType select)
        {
            foreach (var type in select.Selections)
            {
                var nested = type as SelectType;
                if (nested == null)
                {
                    yield return type;
                    continue;
                }
                foreach (var namedType in GetAllSpecific(nested))
                {
                    yield return namedType;
                }
            }
        }

        private static bool IsCompatibleSystemType(BaseType o, BaseType n)
        {
            while (true)
            {
                var targetSelect = n as SelectType;
                var sourceNamed = o as NamedType;
                if (targetSelect != null && sourceNamed != null)
                {
                    var specific = GetAllSpecific(targetSelect);
                    if (specific.Any(
                            s => string.Compare(s.Name, sourceNamed.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                        return !(o is DefinedType);
                }

                //var target = n as DefinedType;
                //var source = o as SimpleType;
                //if (target != null && source != null &&  TypeHelper.GetCSType(target, null) == TypeHelper.GetCSType(source, null))
                //    return true;

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
                {
                    if (
                        (oNamed.Name == "IfcStructuralSurfaceTypeEnum" && nNamed.Name == "IfcStructuralSurfaceMemberTypeEnum") ||
                        (oNamed.Name == "IfcStructuralCurveTypeEnum" && nNamed.Name == "IfcStructuralCurveMemberTypeEnum")
                        )
                        return true;
                    return string.Compare(oNamed.Name, nNamed.Name, StringComparison.InvariantCultureIgnoreCase) == 0;
                }

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
        public ExplicitAttribute TargetAttribute { get; private set; }
        public AttributeMatchType MatchType { get; private set; }
        public string Message { get; private set; }
    }

    public enum AttributeMatchType
    {
        Identity,
        Changed,
        NotFound
    }
}
