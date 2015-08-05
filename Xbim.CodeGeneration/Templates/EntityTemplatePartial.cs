using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

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
            WhereRules = Type.WhereRules.ToList();
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
                    if (!Type.Instantiable)  //avoid redundant inheritance
                        parents.Add(_settings.PersistEntityInterface);
                    parents.Add("INotifyPropertyChanged");
                }
                else
                    parents.AddRange(Type.Supertypes.Select(t => t.Name.ToString()));

                //add any interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //sign types to be instantiable in entity factory
                if(Type.Instantiable)
                    parents.Add(_settings.InstantiableEntityInterface);

                //merge to a single string
                var i = String.Join(", ", parents);
                if (String.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        private string GetCSType(ExplicitAttribute attribute)
        {
            return TypeHelper.GetCSType(attribute, _settings.ItemSetClassName);
        }

        private string ModelInterface { get { return _settings.ModelInterface; } }

        private string AbstractKeyword { get { return IsAbstract ? "abstract" : ""; } }

        private bool IsFirst { get { return Type.Supertypes == null || !Type.Supertypes.Any(); } }
        private bool IsFirstNonAbstract { 
            get 
            { 
                return Type.Instantiable && (Type.Supertypes == null  ||  Type.AllSupertypes.All(t => !t.Instantiable)); 
            } 
        }

        private string InstantiableInterface { get { return _settings.InstantiableEntityInterface; } }

        private string GetPrivateFieldName(Attribute attribute)
        {
            string name = attribute.Name;
            return "_" + name.First().ToString().ToLower() + name.Substring(1);
        }


        private List<ExplicitAttribute> _explAttrCache;

        private List<ExplicitAttribute> ExplicitAttributes
        {
            get { return _explAttrCache ?? (_explAttrCache = Type.ExplicitAttributes.ToList()); }
        }

        private List<ExplicitAttribute> _allExplAttrCache;
        private List<ExplicitAttribute> AllExplicitAttributes
        {
            get { return _allExplAttrCache ?? (_allExplAttrCache = Type.AllExplicitAttributes.ToList()); }
        }

        private List<InverseAttribute> _invAttrCache;
        private List<InverseAttribute> InverseAttributes
        {
            get { return _invAttrCache ?? (_invAttrCache = Type.InverseAttributes.ToList()); }
        }

        private int GetAttributeIndex(ExplicitAttribute attribute)
        { 
            return AllExplicitAttributes.IndexOf(attribute);
        }



        public IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();
                var expl =  IsAbstract ? ExplicitAttributes : AllExplicitAttributes;

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
                    var helper = new NamedTypeHelper(type, _settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if(result.Contains(ns)) continue;
                    result.Add(ns);
                }

                if (iAttributes.Any() || !IsAbstract)
                    result.Add("System.Collections.Generic");

                if (IsFirst)
                {
                    //for INotifyPropertyChanged
                    result.Add("System.ComponentModel");
    
                    //for Action and Exception
                    result.Add("System");
                }

                if (_settings.IsInfrastructureSeparate)
                    result.Add(_settings.InfrastructureNamespace);

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

        private IEnumerable<ExplicitAttribute> AggregatedExplicitAttributes
        {
            get { return ExplicitAttributes.Where(t => t.Domain is AggregationType); }
        }

        private bool IsReferenceTypeAggregation(ExplicitAttribute attribute)
        {
            var agg = attribute.Domain as AggregationType;
            if (agg == null) return false;
            return
                agg.ElementType is EntityDefinition ||
                agg.ElementType is SelectType ||
                agg.ElementType is StringType ||
                agg.ElementType is LogicalType;
        }

        private bool IsValueTypeAggregation(ExplicitAttribute attribute)
        {
            var agg = attribute.Domain as AggregationType;
            if (agg == null) return false;
            return
                agg.ElementType is SimpleType && 
                !(agg.ElementType is StringType ||
                agg.ElementType is LogicalType);
        }

        private bool IsReferenceType(ExplicitAttribute attribute)
        {
            if (attribute.OptionalFlag) return true;

            return
                attribute.Domain is EntityDefinition ||
                attribute.Domain is SelectType ||
                attribute.Domain is StringType ||
                attribute.Domain is LogicalType ||
                attribute.Domain is AggregationType;
        }

        private string GetAggregationElementType(ExplicitAttribute attribute)
        {
            var aggregationType = attribute.Domain as AggregationType;
            if (aggregationType != null)
            {
                var type = aggregationType.ElementType;
                return TypeHelper.GetCSType(type, _settings.ItemSetClassName);
            }
            throw new Exception("Aggregation type expected");
        }

        public List<WhereRule> WhereRules { get; set; }
    }
}
