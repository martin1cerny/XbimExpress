using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class EntityFactoryInterfaceTemplate:ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public EntityFactoryInterfaceTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return _settings.EntityFactoryInterface; }
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
            get
            {
                yield break;
            }
        }

        private string InstantiableEntityInterface { get { return _settings.InstantiableEntityInterface; } }
        private string ModelInterface { get { return _settings.ModelInterface; } }
    }
}
