using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EnumerationTemplate: ICodeTemplate
    {
        public EnumerationType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;

        public EnumerationTemplate(GeneratorSettings settings, EnumerationType type)
        {
            _settings = settings;
            _helper = new NamedTypeHelper(type, settings);
            Type = type;
        }

        public string Namespace
        {
            get
            {
                return _helper.FullNamespace;
            }
        }

        public string Name { get { return Type.Name; } }

        public string Inheritance
        {
            get { return null; }
        }


        public IEnumerable<string> Using
        {
            get { yield break; }
        }
    }
}
