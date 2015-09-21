using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class OptionalItemSetTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public OptionalItemSetTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "Optional" + _settings.ItemSetClassName; }
        }

        public string Namespace
        {
            get { return _settings.Namespace; }
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

        private string ItemSet { get { return _settings.ItemSetClassName; } }
    }
}
