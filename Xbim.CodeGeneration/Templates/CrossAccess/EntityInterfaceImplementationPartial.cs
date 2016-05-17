using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.CodeGeneration.Differences;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.CrossAccess
{
    public partial class EntityInterfaceImplementation
    {
        private readonly EntityDefinitionMatch _match;
        private readonly List<EntityDefinitionMatch> _matches;
        private readonly List<ExplicitAttributeMatch> _superMatches = new List<ExplicitAttributeMatch>();
        public EntityDefinition RemoteType { get; private set; }

        private bool _implementsSupertype;

        public EntityInterfaceImplementation(GeneratorSettings settings, EntityDefinitionMatch match, List<EntityDefinitionMatch> matches) : base(settings, match.Source)
        {
            _match = match;
            _matches = matches;
            RemoteType = match.Target;
        }

        protected bool IsDirectTypeMatch(ExplicitAttributeMatch match)
        {
            var source = match.SourceAttribute.Domain as EntityDefinition;
            var target = match.TargetAttribute.Domain as EntityDefinition;
            if (source == null || target == null) return false;

            return _matches.Any(m => m.Source == source && m.Target == target);
        }

        protected ExplicitAttributeMatch GetMatch(ExplicitAttribute remoteAttribute)
        {
            var match = _match.AttributeMatches.FirstOrDefault(m => m.TargetAttribute == remoteAttribute);
            if (match == null && _implementsSupertype)
            {
                match = _superMatches.FirstOrDefault(m => m.TargetAttribute == remoteAttribute);
            }
            return match;
        }

        protected bool IsNew(ExplicitAttribute remoteAttribute)
        {
            var hierarchy = _match.Target.AllSupertypes.Select(t => t.Name).Union(new [] {_match.Target.Name}).ToList();
            var names = NewTargetAttributes.Where(t => hierarchy.Any(i => t.Item1 == i)).Select(t => t.Item2).ToList();
            return names.Any() && 
                names.Any(n => string.Equals(n, remoteAttribute.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        private static readonly List<Tuple<string, string>> NewTargetAttributes = new List<Tuple<string, string>>();

        static EntityInterfaceImplementation()
        {
            var reader = new StringReader(Properties.Resources.Ifc4_NewProperties);
            var line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('.').Select(i => i.Trim()).ToList();
                NewTargetAttributes.Add(new Tuple<string, string>(parts[0], parts[1]));
                line = reader.ReadLine();
            }
        }

        protected string GetBaseSystemType(DefinedType type)
        {
            return TypeHelper.GetCSType(type.Domain, Settings);
        }

        protected string GetBaseSystemType(SimpleType type)
        {
            return TypeHelper.GetCSType(new DefinedType{ Domain = type}, Settings);
        }

        protected EnumerationType GetMappedEnumerationType(EnumerationType source)
        {
            var tModel = _match.Target.SchemaModel;

            var nameMatch =
                tModel.Get<EnumerationType>(
                    t => string.Compare(source.Name, t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralSurfaceTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralSurfaceMemberTypeEnum").FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralCurveTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralCurveMemberTypeEnum").FirstOrDefault();

            return nameMatch;
        }


        protected string GetEnumEquivalent(string value, EnumerationType target)
        {
            var normValue = NormalizeString(value);

            var identity =
                target.Elements.FirstOrDefault(
                    e => string.Compare(normValue, NormalizeString(e), StringComparison.InvariantCultureIgnoreCase) == 0);
            if (identity != default(ExpressId))
                return identity;

            //var ndef =
            //    target.Elements.FirstOrDefault(
            //        e => string.Compare("NOTDEFINED", NormalizeString(e), StringComparison.InvariantCultureIgnoreCase) == 0);
            //if (ndef != default(ExpressId))
            //    return ndef;

            return null;
        }

        private string NormalizeString(string input)
        {
            return input.Replace("_", "").ToUpperInvariant();
        }

        protected bool IsInSelect(NamedType type, SelectType select)
        {
            var allSpecific = GetAllSpecific(select).ToList();
            if (allSpecific.Any(s => string.Compare(s.Name, type.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                return true;

            var entity = type as EntityDefinition;
            if (entity == null) return false;
            
            var supertypes = entity.AllSupertypes.ToList();
            var matches = _matches.Where(m => supertypes.Any(s => s == m.Source) || entity == m.Source).ToList();
            return allSpecific.Any(s => matches.Any(m => s == m.Target));
        }

        protected DefinedType GetMappedDefinedType(DefinedType source)
        {
            var tModel = _match.Target.SchemaModel;

            var nameMatch =
                tModel.Get<DefinedType>(
                    t => string.Compare(source.Name, t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
            return nameMatch;
        }

        protected IEnumerable<NamedType> GetAllSpecific(SelectType select)
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

        protected IEnumerable<ExplicitAttribute> ExplicitAttributesToImplement
        {
            get
            {
                foreach (var attribute in RemoteType.ExplicitAttributes)
                {
                    yield return attribute;
                }
                //if supertype is not implemented in the inheritance chain it must be implemented down in hierarchy
                if(RemoteType.Supertypes != null && RemoteType.Supertypes.Any())
                    foreach (var supertype in RemoteType.AllSupertypes)
                    {
                        if(_matches.Where(m => m.Target == supertype).Any(m => Type.AllSupertypes.Any(s => s == m.Source)))
                            continue;
                        foreach (var attribute in supertype.ExplicitAttributes)
                        {
                            yield return attribute;
                        }
                        _implementsSupertype = true;
                        _superMatches.AddRange(ExplicitAttributeMatch.FindMatches(_match.Source, supertype));
                    }
            }
        }

        protected IEnumerable<DerivedAttribute> DerivedAttributesToImplement
        {
            get
            {
                foreach (var attribute in RemoteType.DerivedAttributes.Where(a => a.Redeclaring == null && !IsOverwritting(a)))
                {
                    yield return attribute;
                }
                //if supertype is not implemented in the inheritance chain it must be implemented down in hierarchy
                if (RemoteType.Supertypes != null && RemoteType.Supertypes.Any())
                    foreach (var supertype in RemoteType.AllSupertypes)
                    {
                        if (_matches.Where(m => m.Target == supertype).Any(m => Type.AllSupertypes.Any(s => s == m.Source)))
                            continue;
                        foreach (var attribute in supertype.DerivedAttributes.Where(a => a.Redeclaring == null && !IsOverwritting(a)))
                        {
                            yield return attribute;
                        }
                        _implementsSupertype = true;
                    }
            }
        }


        protected IEnumerable<InverseAttribute> InverseAttributesToImplement
        {
            get
            {
                foreach (var attribute in RemoteType.InverseAttributes)
                {
                    yield return attribute;
                }
                if (RemoteType.Supertypes != null && RemoteType.Supertypes.Any())
                    foreach (var supertype in RemoteType.AllSupertypes)
                    {
                        if (_matches.Where(m => m.Target == supertype).Any(m => Type.AllSupertypes.Any(s => s == m.Source)))
                            continue;
                        foreach (var attribute in supertype.InverseAttributes)
                        {
                            yield return attribute;
                        }
                        _implementsSupertype = true;
                    }
            }
        }


        protected string OwnNamespace { get { return base.Namespace; } }

        /// <summary>
        /// Overriden so that the actual file is separate. This namespace is not used in 
        /// the generated partial class.
        /// </summary>
        public override string Namespace
        {
            get { return string.Format("{0}.{1}.{2}", Settings.Namespace, Settings.SchemaInterfacesNamespace, RemoteType.ParentSchema.Name); }
        }

        protected string GetDerivedAttributePlacement(DerivedAttribute attribute)
        {
            var type = attribute.ParentEntity;
            if (Settings.IgnoreDerivedAttributes == null || !Settings.IgnoreDerivedAttributes.Any())
                return string.Format("I{0}", attribute.ParentEntity.Name);

            var select = type.IsInAllSelects.FirstOrDefault(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if (select != null)
            {
                var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
                var ns = GetFullNamespace(select, baseNamespace, Settings.CrossAccessStructure);
                ns = TrimNamespace(ns);
                return string.Format("{0}.{1}", ns, select.Name);
            }

            var entity = type.AllSupertypes.FirstOrDefault(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if(entity != null)
                return string.Format("I{0}", entity.Name);

            return string.Format("I{0}", attribute.ParentEntity.Name);
        }

        protected bool IsDirectDerived(DerivedAttribute attribute)
        {
            var domain = attribute.Domain;
            var aggr = domain as AggregationType;
            while (aggr != null)
            {
                domain = aggr.ElementType;
                aggr = domain as AggregationType;
            }
            if (domain is SimpleType) return true;
            var named = domain as EntityDefinition;
            if (named == null) return false;
            return SpecialDerivesDictionary.Keys.Any(s => string.Compare(s, named.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        private string TrimNamespace(string fullName)
        {
            var own = OwnNamespace.Split('.');
            var name = fullName.Split('.').ToList();
            foreach (var part in own)
            {
                if (string.CompareOrdinal(name[0], part) == 0)
                {
                    name.RemoveAt(0);
                    continue;
                }
                break;
            }
            return string.Join(".", name);
        }

        protected string GetInterfaceCSTypeFull(ExplicitAttribute attribute)
        {
            var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
            var fullName = TypeHelper.GetInterfaceCSType(attribute, Settings, GetFullNamespace(attribute.Domain, baseNamespace, Settings.CrossAccessStructure));
            return TrimNamespace(fullName);
        }

        protected string GetInterfaceCSTypeFull(DerivedAttribute attribute)
        {
            var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
            var fullName = TypeHelper.GetCSType(attribute.Domain, Settings,true, true, GetFullNamespace(attribute.Domain, baseNamespace, Settings.CrossAccessStructure));
            fullName = TweekDerivedType(attribute, fullName);
            return TrimNamespace(fullName);
        }

        protected string GetInterfaceCSTypeFull(NamedType type)
        {
            var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
            var fullName = TypeHelper.GetCSType(type, Settings, false, true, GetFullNamespace(type, baseNamespace, Settings.CrossAccessStructure));
            return TrimNamespace(fullName);
        }

        protected string GetCSTypeFull(ExplicitAttribute attribute)
        {
            var fullName = TypeHelper.GetCSType(attribute.Domain, Settings, false, false, GetFullNamespace(attribute.Domain, Settings.Namespace, Settings.Structure));
            return TrimNamespace(fullName);
        }

        protected string GetCSTypeFull(NamedType type)
        {
            var fullName = TypeHelper.GetCSType(type, Settings, false, false, GetFullNamespace(type, Settings.Namespace, Settings.Structure));
            return TrimNamespace(fullName);
        }

        protected string Interface { get { return "I" + RemoteType.Name; } }

        public override IEnumerable<string> Using
        {
            get 
            {
                var usings = new List<string>
                {
                    Settings.CrossAccessNamespace, 
                    "System.Collections.Generic",
                    "System.Linq"
                };

                return usings.Distinct();
            }
        }
    }
}
