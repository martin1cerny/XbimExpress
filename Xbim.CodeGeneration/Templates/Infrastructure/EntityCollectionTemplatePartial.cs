using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class EntityCollectionTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public EntityCollectionTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return _settings.EntityCollentionInterface; }
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

        private string PersistEntityInterface { get { return _settings.PersistEntityInterface; } }
    }
}
