using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class OptionalItemSetInterfaceTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public OptionalItemSetInterfaceTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public string Name
        {
            get { return "IOptional" + _settings.ItemSetClassName; }
        }

        public string Namespace
        {
            get { return _settings.InfrastructureNamespace; }
        }

        public string Inheritance
        {
            get { return string.Format(": I{0}<T>", _settings.ItemSetClassName); }
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
    }
}
