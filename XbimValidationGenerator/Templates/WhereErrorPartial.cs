using System.Collections.Generic;
using XbimValidationGenerator.Schema;

namespace XbimValidationGenerator.Templates
{
    partial class WhereError: ICodeTemplate
    {
        private readonly string _schema;
        public TypeRules Rules { get; private set; }
        public string Name { get { return Rules.Type + "Errors"; } }
        public string Namespace { get { return Settings.BaseNamespace + ".Errors." + _schema; } }

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

        public WhereError(TypeRules rules, string schema)
        {
            _schema = schema;
            Rules = rules;
        }
    }
}
