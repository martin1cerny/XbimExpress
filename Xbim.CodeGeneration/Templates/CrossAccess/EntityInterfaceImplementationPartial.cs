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
        public EntityDefinition RemoteType { get; }

        private bool _implementsSupertype;

        public EntityInterfaceImplementation(GeneratorSettings settings, EntityDefinitionMatch match, List<EntityDefinitionMatch> matches) : base(settings, match.Source)
        {
            _match = match;
            _matches = matches;
            RemoteType = match.Target;
            AllRemoteExplicitAttributes = RemoteType.AllExplicitAttributes.ToList();
        }

        protected IList<ExplicitAttribute> AllRemoteExplicitAttributes { get; }

        protected int GetRemoteOrder(ExplicitAttribute attribute)
        {
            var idx = AllRemoteExplicitAttributes.IndexOf(attribute);
            return idx < 0 ? -1 : idx + 1;
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
            var tModel = _match.Target.SchemaModel == source.SchemaModel ?
                _match.Source.SchemaModel :
                _match.Target.SchemaModel;

            var nameMatch =
                tModel.Get<EnumerationType>(
                    t => string.Compare(source.Name, t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralSurfaceTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralSurfaceMemberTypeEnum").FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralSurfaceMemberTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralSurfaceTypeEnum").FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralCurveTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralCurveMemberTypeEnum").FirstOrDefault();
            if (nameMatch == null && source.Name == "IfcStructuralCurveMemberTypeEnum")
                return tModel.Get<EnumerationType>(t => t.Name == "IfcStructuralCurveTypeEnum").FirstOrDefault();

            return nameMatch;
        }


        protected string GetEnumEquivalent(string value, EnumerationType target)
        {
            var normValue = NormalizeString(value);

            var identity =
                target.Elements.FirstOrDefault(
                    e => string.Compare(normValue, NormalizeString(e), StringComparison.InvariantCultureIgnoreCase) == 0);
            return identity != default(ExpressId) ? identity : null;
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
            
            var tModel = _match.Target.SchemaModel == source.SchemaModel ? 
                _match.Source.SchemaModel : 
                _match.Target.SchemaModel;

            if (source.Name == "IfcSoundPowerLevelMeasure")
                return tModel.Get<DefinedType>(
                    t => string.Compare("IfcSoundPowerMeasure", t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
            if (source.Name == "IfcSoundPressureLevelMeasure")
                return tModel.Get<DefinedType>(
                    t => string.Compare("IfcSoundPressureMeasure", t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();

            var nameMatch =
                tModel.Get<DefinedType>(
                    t => string.Compare(source.Name, t.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    .FirstOrDefault();
            return nameMatch;
        }

        protected IEnumerable<NamedType> GetAllSpecificNative(SelectType select)
        {
            var remotes = GetAllSpecific(select).ToList();
            var remoteNames = remotes.Select(s => s.Name).ToList();

            var entities = _matches.Where(m => m.Target != null && remoteNames.Any(r => m.Target.Name == r)).Select(m => m.Source);
            var types = remotes.OfType<DefinedType>();

            return new NamedType[0]
                .Concat(entities)
                .Concat(types);
        }

        protected bool IsAggregationEntityCompatible(ExplicitAttribute source, ExplicitAttribute target)
        {
            if (source.Domain.GetType() != target.Domain.GetType())
                return false;

            var sType = source.Domain;
            var tType = target.Domain;

            if (sType is AggregationType)
            {
                sType = ((AggregationType) sType).ElementType;
                tType = ((AggregationType) tType).ElementType;
            }

            var sEntity = sType as EntityDefinition;
            var tEntity = tType as EntityDefinition;
            if (sEntity != null && tEntity != null)
            {
                return _matches.Any(m => m.Source == sEntity && m.Target == tEntity);
            }

            var sSelect = sType as SelectType;
            var tSelect = tType as SelectType;
            if (sSelect != null)
            {
                return IsSelectCompatible(sSelect, tSelect, _matches);
            }
            return false;
        }

        internal static bool IsSelectCompatible(SelectType source, SelectType target, IEnumerable<EntityDefinitionMatch> matches)
        {
            var sImpls = GetAllSpecific(source).ToList();
            var tImpls = GetAllSpecific(target).Where(i => matches.Any(m => m.Target == i && m.Source != null)).ToList();
            if (sImpls.All(s => s is EntityDefinition) && tImpls.All(s => s is EntityDefinition))
            {
                return 
                    tImpls.Count == sImpls.Count && 
                    sImpls.All(t =>
                    {
                        var match = matches.FirstOrDefault(m => m.Source == t);
                        return match != null && tImpls.Any(s => s == match.Target);
                    });
            }
            return false;
        }

        protected IEnumerable<NamedType> GetRemovedTypes(SelectType o, SelectType n)
        {
            var oSpecific = GetAllSpecific(o);
            var nSpecific = GetAllSpecific(n).ToList();
            return oSpecific.Where(s => nSpecific.All(ns => ns.Name != s.Name));
        }

        protected IEnumerable<NamedType> GetAddedTypes(SelectType o, SelectType n)
        {
            var oSpecific = GetAllSpecific(o).ToList();
            var nSpecific = GetAllSpecific(n).ToList();
            return nSpecific.Where(s => oSpecific.All(ns => ns.Name != s.Name));
        }

        protected IEnumerable<DefinedType> GetAddedDefinedTypes(SelectType o, SelectType n)
        {
            return GetAddedTypes(o, n).OfType<DefinedType>();
        }

        protected bool IsEntitySelection(SelectType select)
        {
            return GetAllSpecific(select).All(s => s is EntityDefinition);
        }

        protected IEnumerable<EntityDefinition> GetAddedEntities(SelectType o, SelectType n)
        {
            var candidates = GetAddedTypes(o,n).OfType<EntityDefinition>().Select(c => c.Name).ToList();
            //find equivalents in old schema
            return _matches.Where(m => m.Target != null && candidates.Any(c => c == m.Target.Name)).Select(m => m.Source);
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


        protected string OwnNamespace => base.Namespace;

        /// <summary>
        /// Overriden so that the actual file is separate. This namespace is not used in 
        /// the generated partial class.
        /// </summary>
        public override string Namespace =>
            $"{Settings.Namespace}.{Settings.SchemaInterfacesNamespace}.{RemoteType.ParentSchema.Name}";

        protected string GetDerivedAttributePlacement(DerivedAttribute attribute)
        {
            var type = attribute.ParentEntity;
            if (Settings.IgnoreDerivedAttributes == null || !Settings.IgnoreDerivedAttributes.Any())
                return $"I{attribute.ParentEntity.Name}";

            var select = type.IsInAllSelects.FirstOrDefault(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if (select != null)
            {
                var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
                var ns = GetFullNamespace(select, baseNamespace, Settings.CrossAccessStructure);
                ns = TrimNamespace(ns);
                return $"{ns}.{@select.Name}";
            }

            var entity = type.AllSupertypes.FirstOrDefault(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if(entity != null)
                return $"I{entity.Name}";

            return $"I{attribute.ParentEntity.Name}";
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

        protected bool IsSimpleTypeCompatible(ExplicitAttribute n, ExplicitAttribute o)
        {
            if (n.OptionalFlag != o.OptionalFlag)
                return false;

            var s = n.Domain as SimpleType ?? o.Domain as SimpleType;
            var d = n.Domain as DefinedType ?? o.Domain as DefinedType;
            if (s == null || d == null)
                return false;

            var nT = TypeHelper.GetCSType(s, null);
            var oT = TypeHelper.GetCSType(d.Domain, null);
            return oT == nT;

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

        protected string Interface => "I" + RemoteType.Name;

        public override IEnumerable<string> Using
        {
            get 
            {
                var usings = new List<string>
                {
                    Settings.CrossAccessNamespace, 
                    "System.Collections.Generic",
                    "System.Linq",
                    "Xbim.Common"
                };

                return usings.Distinct();
            }
        }
    }
}
