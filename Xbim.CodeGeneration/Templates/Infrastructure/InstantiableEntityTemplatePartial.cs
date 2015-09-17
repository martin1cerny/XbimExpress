using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class InstantiableEntityTemplate:ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public InstantiableEntityTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return _settings.InstantiableEntityInterface; }
        }

        public string Namespace
        {
            get { return _settings.InfrastructureNamespace; }
        }

        public string Inheritance
        {
            get { return string.Format(": {0}", _settings.PersistEntityInterface); }
        }

        public IEnumerable<string> Using
        {
            get { yield break; }
        }
    }
}
