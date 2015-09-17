using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class ExpressTypeInterfaces: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public ExpressTypeInterfaces(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        public string Namespace
        {
            get { return _settings.InfrastructureNamespace; }
        }

        public string Inheritance
        {
            get { return ""; }
        }

        public IEnumerable<string> Using
        {
            get { yield break; }
        }

        private string PersistInterface { get { return _settings.PersistInterface; } }

        private string InstantiableInterface { get { return _settings.InstantiableEntityInterface; } }
    }
}
