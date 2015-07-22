using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class TransactionTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;

        public TransactionTemplate(GeneratorSettings settings)
        {
            _settings = settings;
        }


        public string Name
        {
            get { return _settings.TransactionInterface; }
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
            get { yield break; }
        }
    }
}
