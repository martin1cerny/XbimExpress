using System.Text.RegularExpressions;
using XbimValidationGenerator.Schema;
// ReSharper disable UnusedMember.Local

namespace XbimValidationGenerator.Templates
{
    public partial class SchemaRule:ICodeTemplate
    {
        private readonly string _schema;
        private string Schema { get { return char.ToUpper(_schema[0]) + _schema.Substring(1).ToLower(); } }
        private string SchemaNamespace { get { return "Xbim." + Schema; } }
        public TypeRules Rules { get; private set; }

        public string Name
        {
            get { return Rules.Type + "Rules"; }
        }

        public string Namespace
        {
            get { return Settings.BaseNamespace + ".Rules." + _schema; }
        }

        public string Inheritance
        {
            get { return ""; }
        }

        public System.Collections.Generic.IEnumerable<string> Using
        {
            get
            {
                yield return "System";
                yield return "System.Linq";
                yield return "System.Collections.Generic";
                yield return "Xbim.Common";
                yield return "Xbim.Validation.Errors";
                yield return string.Format("{0}.{1}", SchemaNamespace, Settings.Structure.GetDomainForType(Rules.Type).Name);
            }
        }

        private string GetDefinition(WhereRule rule)
        {
            return new Regex("^", RegexOptions.Multiline).Replace(rule.Definition, "            // ");
        }

        private string GetDescription(WhereRule rule)
        {
            return new Regex("^", RegexOptions.Multiline).Replace(rule.Description, "        /// ");
        }

        public SchemaRule(TypeRules rules, string schema)
        {
            _schema = schema;
            Rules = rules;
        }

        private string GetErr(WhereRule rule)
        {
            return string.Format("{0}Errors.{1}.{2}", Schema, Rules.Type, rule.Name);
        }

        private bool GetElementType(WhereRule rule, out string type)
        {
            type = new Regex("Ifc\\w+Type").Match(rule.Description).Value;
            return !string.IsNullOrWhiteSpace(type);
        }

        private bool GetPredefinedType(WhereRule rule, out string type)
        {
            type = new Regex("(?<type>Ifc\\w+)\\.USERDEFINED").Match(rule.Definition).Groups["type"].Value;
            return !string.IsNullOrWhiteSpace(type);
        }

        private string GetUserDefinedType(WhereRule rule)
        {
            return new Regex("EXISTS\\s*\\([^\\)]+?\\.(?<type>.+?)\\)").Match(rule.Definition).Groups["type"].Value;
        }

        private string GetPredefinedTypeNamespace(string type)
        {
            return string.Join(".", Schema, Schema != "Ifc4" ? Settings.Structure.GetDomainForType(type).Name : "Interfaces");
        }
    }
}
