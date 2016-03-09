using System.Collections.Generic;
using XbimValidationGenerator.Schema;

namespace XbimValidationGenerator.Templates
{
    partial class SchemaErrors: ICodeTemplate
    {
        public SchemaRules Schema { get; private set; }

        public string Name
        {
            get { return char.ToUpper(Schema.Schema[0]) + Schema.Schema.Substring(1).ToLower() + "Errors"; }
        }

        public string Namespace
        {
            get { return Settings.BaseNamespace + ".Errors"; }
        }

        public string Inheritance
        {
            get { return ""; }
        }

        public IEnumerable<string> Using
        {
            get { yield return Settings.BaseNamespace + ".Errors." + Schema.Schema; }
        }

        public SchemaErrors(SchemaRules schema)
        {
            Schema = schema;
        }
    }
}
