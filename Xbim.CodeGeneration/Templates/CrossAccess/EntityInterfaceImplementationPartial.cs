using System;
using System.Collections.Generic;
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
        public EntityDefinition RemoteType { get; private set; }

        public EntityInterfaceImplementation(GeneratorSettings settings, EntityDefinitionMatch match, List<EntityDefinitionMatch> matches) : base(settings, match.Source)
        {
            _match = match;
            _matches = matches;
            RemoteType = match.Target;
        }

        protected ExplicitAttributeMatch GetMatch(ExplicitAttribute remoteAttribute)
        {
            return _match.AttributeMatches.FirstOrDefault(m => m.TargetAttribute == remoteAttribute);
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
                if(RemoteType.Supertypes != null && RemoteType.Supertypes.Any())
                    foreach (var supertype in RemoteType.AllSupertypes)
                    {
                        if(_matches.Where(m => m.Target == supertype).Any(m => Type.AllSupertypes.Any(s => s == m.Source)))
                            continue;
                        foreach (var attribute in supertype.ExplicitAttributes)
                        {
                            yield return attribute;
                        }
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

        protected string GetInterfaceCSTypeFull(ExplicitAttribute attribute)
        {
            var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
            return TypeHelper.GetInterfaceCSType(attribute, Settings, GetFullNamespace(attribute.Domain, baseNamespace, Settings.CrossAccessStructure));
        }

        protected string GetInterfaceCSTypeFull(NamedType type)
        {
            var baseNamespace = Settings.CrossAccessNamespace.Replace("." + Settings.SchemaInterfacesNamespace, "");
            return TypeHelper.GetCSType(type, Settings, false, true, GetFullNamespace(type, baseNamespace, Settings.CrossAccessStructure));
        }

        protected string GetCSTypeFull(ExplicitAttribute attribute)
        {
            return TypeHelper.GetCSType(attribute.Domain, Settings, false, false, GetFullNamespace(attribute.Domain, Settings.Namespace, Settings.Structure));
        }

        protected string GetCSTypeFull(NamedType type)
        {
            return TypeHelper.GetCSType(type, Settings, false, false, GetFullNamespace(type, Settings.Namespace, Settings.Structure));
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
