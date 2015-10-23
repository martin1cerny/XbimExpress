using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.ExpressParser.SDAI;

namespace XbimTranslatorGenerator.Differences
{
    /// <summary>
    /// This class contains information about entity definition match for instantiable entities
    /// </summary>
    public class EntityDefinitionMatch
    {
        /// <summary>
        /// Constructor will try to find matching definition from target schema
        /// </summary>
        /// <param name="source">Instantiable entity definition</param>
        /// <param name="targetSchema">Target schema where target entity definition should be found</param>
        public EntityDefinitionMatch(EntityDefinition source, SchemaModel targetSchema)
        {
            if(!source.Instantiable)
                throw new ArgumentException(@"Source should be instantiable entity definition", "source");

            Source = source;
            //default state unless we find a match
            MatchType = EntityMatchType.NotFound;

            //try to find identity
            var identity = targetSchema.Get<EntityDefinition>(
                e => string.Compare(source.Name, e.Name, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
            if (identity != null && identity.Instantiable)
            {
                Target = identity;
                var attrMatches = ExplicitAttributeMatch.FindMatches(Source, Target).ToList();
                AttributeMatches = attrMatches;
                MatchType = attrMatches.Any(am => am.MatchType != AttributeMatchType.Identity)? 
                    EntityMatchType.Changed : 
                    EntityMatchType.Identity;
                return;
            }

            //try to find corresponding parent type if this doesn't have any explicit attributes
            var superTypeMatch = GetNonAbstractSupertypeMatch(source, targetSchema);
            if (superTypeMatch != null)
            {
                Target = superTypeMatch;
                MatchType = EntityMatchType.Changed;
                AttributeMatches = ExplicitAttributeMatch.FindMatches(Source, Target).ToList();
                return;
            }

            //try to find a match based on the sequence of all explicit attribute names
            var attrSigniture = string.Join(", ", source.AllExplicitAttributes.Select(a => a.Name));
            var attrMatch = targetSchema.Get<EntityDefinition>(
                e => string.Compare(attrSigniture, string.Join(", ", e.AllExplicitAttributes.Select(a => a.Name)), StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
            if (attrMatch != null)
            {
                Target = attrMatch;
                MatchType = EntityMatchType.Changed;
                AttributeMatches = ExplicitAttributeMatch.FindMatches(Source, Target).ToList();
                return;
            }
        }

        private static EntityDefinition GetNonAbstractSupertypeMatch(EntityDefinition entity, SchemaModel targetSchema)
        {
            //supertype has to have the same number of attributes so de don't loose any data
            var attrCount = entity.AllExplicitAttributes.Count();
            while (entity.AllExplicitAttributes.Count() == attrCount)
            {
                var supertype = entity.Supertypes.FirstOrDefault();
                if (supertype == null) return null;
                if (supertype.Instantiable)
                {
                    var candidate = targetSchema.Get<EntityDefinition>(
                        e => string.Compare(supertype.Name, e.Name, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
                    return candidate;
                }
                entity = supertype;
            }
            return null;
        }

        public EntityDefinition Source { get; private set; }
        public EntityDefinition Target { get; private set; }
        public EntityMatchType MatchType { get; private set; }

        public IEnumerable<ExplicitAttributeMatch> AttributeMatches { get; private set; } 
    }

    public enum EntityMatchType
    {
        Identity,
        Changed,
        NotFound
    }
}
