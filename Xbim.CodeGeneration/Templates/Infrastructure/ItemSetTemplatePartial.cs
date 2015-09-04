using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class ItemSetTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public ItemSetTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return _settings.ItemSetClassName; }
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
                var result = new List<string>();
                if (_settings.IsInfrastructureSeparate)
                    result.Add(_settings.InfrastructureNamespace);
                return result;
            }
        }

        private string ModelInterface { get { return _settings.ModelInterface; } }
    }
}
