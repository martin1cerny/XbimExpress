using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityInterfaceTemplate
    {
        public EntityInterfaceTemplate(GeneratorSettings settings, EntityDefinition type): base(settings, type)
        {
            
        }

        public override IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string> {"System.Collections.Generic", "System.ComponentModel", "System"};
                if (InverseAttributes.Any(IsDoubleAggregation))
                    result.Add("System.Linq");

                var namedOccurances = new List<NamedType>();
                var expl = IsAbstract ? ExplicitAttributes : AllExplicitAttributes;

                var selects = Type.IsInSelects.ToList();
                var supertypes = Type.Supertypes ?? new HashSet<EntityDefinition>();
                var eAttributes = expl.Where(a => a.Domain is NamedType).Select(a => a.Domain as NamedType).ToList();
                var eaAttributes =
                    expl.Where(a => a.Domain is AggregationType)
                        .Select(a => GetNamedElementType(a.Domain as AggregationType))
                        .Where(t => t != null).ToList();
                var iAttributes = InverseAttributes.Select(a => a.Domain).ToList();

                namedOccurances.AddRange(selects);
                namedOccurances.AddRange(supertypes);
                namedOccurances.AddRange(eAttributes);
                namedOccurances.AddRange(eaAttributes);
                namedOccurances.AddRange(iAttributes);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, Settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                if (Settings.IsInfrastructureSeparate)
                    result.Add(Settings.InfrastructureNamespace);

                return result;
            }
        }
    }
}
