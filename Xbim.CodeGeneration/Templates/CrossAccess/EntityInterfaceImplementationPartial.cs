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
        private readonly List<EntityDefinitionMatch> _matches;
        public EntityDefinition RemoteType { get; private set; }

        public EntityInterfaceImplementation(GeneratorSettings settings, EntityDefinitionMatch match, List<EntityDefinitionMatch> matches) : base(settings, match.Source)
        {
            _matches = matches;
            RemoteType = match.Target;
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

        protected string Interface { get { return "I" + RemoteType.Name; } }

        public override IEnumerable<string> Using
        {
            get 
            {
                var usings = new List<string>
                {
                    Settings.CrossAccessNamespace, 
                    "System.Collections.Generic"
                };

                return usings.Distinct();
            }
        }
    }
}
