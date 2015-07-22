using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityTemplate : ICodeTemplate
    {
        public EntityDefinition Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;

        public EntityTemplate(GeneratorSettings settings, EntityDefinition type)
        {
            _settings = settings;
            _helper = new NamedTypeHelper(type, settings);
            Type = type;
        }

        public string Namespace
        {
            get
            {
                return _helper.FullNamespace;
            }
        }

        public bool IsAbstract { get { return !Type.Instantiable; } }

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get
            {
                var parents = new List<string>();
                if (IsFirst)
                {
                    parents.Add(_settings.PersistEntityInterface);
                }
                else
                    parents.AddRange(Type.Supertypes.Select(t => t.Name.ToString()));

                //add any interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //merge to a single string
                var i = String.Join(", ", parents);
                if (String.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        private string ModelInterface { get { return _settings.ModelInterface; } }

        private string AbstractKeyword { get { return IsAbstract ? "abstract" : ""; } }

        private bool IsFirst { get { return Type.Supertypes == null || !Type.Supertypes.Any(); } }

        private string GetPrivateFieldName(ExplicitAttribute attribute)
        {
            string name = attribute.Name;
            return "_" + name.First().ToString().ToLower() + name.Substring(1);
        }


        private List<ExplicitAttribute> _attrCache; 
        private int GetAttributeIndex(ExplicitAttribute attribute)
        { 
            if (_attrCache == null) _attrCache = Type.AllExplicitAttributes.ToList();
            return _attrCache.IndexOf(attribute);
        }


        public IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();
                var expl = Type.ExplicitAttributes.ToList();

                var selects = Type.IsInSelects.ToList();
                var supertypes = Type.Supertypes ?? new HashSet<EntityDefinition>();
                var eAttributes = expl.Where(a => a.Domain is NamedType).Select(a => a.Domain as NamedType).ToList();
                var eaAttributes =
                    expl.Where(a => a.Domain is AggregationType)
                        .Select(a => GetNamedElementType(a.Domain as AggregationType))
                        .Where(t => t != null).ToList();
                var iAttributes = Type.InverseAttributes.Select(a => a.Domain).ToList();

                namedOccurances.AddRange(selects);
                namedOccurances.AddRange(supertypes);
                namedOccurances.AddRange(eAttributes);
                namedOccurances.AddRange(eaAttributes);
                namedOccurances.AddRange(iAttributes);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, _settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if(result.Contains(ns)) continue;
                    result.Add(ns);
                }

                if (iAttributes.Any() || expl.Any(a => a.Domain is AggregationType))
                    result.Add("System.Collections.Generic");

                return result;
            }
        }

        private NamedType GetNamedElementType(AggregationType aggregation)
        {
            if (aggregation.ElementType is SimpleType) return null;

            var named = aggregation.ElementType as NamedType;
            if (named != null) return named;

            var aggr = aggregation.ElementType as AggregationType;
            if(aggr != null)
                return GetNamedElementType(aggr);

            throw new NotSupportedException();
        }

        private string PersistEntityInterface { get { return _settings.PersistEntityInterface; } }

        private bool IsAggregation(InverseAttribute attribute)
        {
            return attribute.InvertedAttr.Domain is AggregationType;
        }
    }
}
