using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Differences
{
    /// <summary>
    /// This class contains information about entity definition match for instantiable entities
    /// </summary>
    public class EntityDefinitionMatch
    {
        public static IEnumerable<EntityDefinitionMatch> GetMatches(SchemaModel a, SchemaModel b)
        {
            return a.Get<EntityDefinition>().Select(entityDefinition => new EntityDefinitionMatch(entityDefinition, b));
        }

        public string MatchName { get; private set; }

        /// <summary>
        /// Constructor will try to find matching definition from target schema
        /// </summary>
        /// <param name="source">Entity definition</param>
        /// <param name="targetSchema">Target schema where target entity definition should be found</param>
        public EntityDefinitionMatch(EntityDefinition source, SchemaModel targetSchema)
        {
            MatchName = string.Format("{0}To{1}", source.ParentSchema.Name, targetSchema.FirstSchema.Name);
            Source = source;
            //default state unless we find a match
            MatchType = EntityMatchType.NotFound;
            AttributeMatches = Enumerable.Empty<ExplicitAttributeMatch>();

            //try to find identity
            var identity = targetSchema.Get<EntityDefinition>(
                e => string.Compare(source.Name, e.Name, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
            if (identity != null)
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
            var superTypeMatch = GetSupertypeMatch(source, targetSchema);
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
            }
        }

        private static EntityDefinition GetSupertypeMatch(EntityDefinition entity, SchemaModel targetSchema)
        {
            while (true)
            {
                if(entity.Supertypes == null || !entity.Supertypes.Any())
                    return null;
                var supertype = entity.Supertypes.First();
                var candidate = targetSchema.Get<EntityDefinition>(
                    e => string.Compare(supertype.Name, e.Name, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
                if (candidate != null)
                    return candidate;

                entity = supertype;
            }
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
