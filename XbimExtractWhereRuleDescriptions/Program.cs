using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using XbimValidationGenerator.Schema;

namespace XbimExtractWhereRuleDescriptions
{
    internal class Program
    {
        private static void Main()
        {
            const string rootDir = @"c:\CODE\IFC4_ADD1\schema";
            const string expressFile = @"C:\CODE\XbimGit\XbimExpress\Xbim.ExpressParser\ExpressDefinitions\IFC4_ADD1.txt";
            const string schema = "IFC4";

            //const string rootDir = @"c:\CODE\IFC2x3_TC1";
            //const string expressFile = @"C:\CODE\XbimGit\XbimExpress\Xbim.ExpressParser\ExpressDefinitions\IFC2X3_TC1.txt";
            //const string schema = "IFC2X3";

            var sRules = new SchemaRules
            {
                Schema = schema,
                TypeRulesSet = new List<TypeRules>()
            };


            var expressData = File.ReadAllText(expressFile);
            foreach (
                var dir in
                    Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly))
            {
                var subdir = Path.Combine(dir, "lexical");
                if (!Directory.Exists(subdir)) continue;

                foreach (
                    var file in
                        Directory.GetFiles(subdir, "*.htm", SearchOption.TopDirectoryOnly))
                {

                    var data = File.ReadAllText(file);
                    //remove all end lines and multiple spaces
                    data = data.Replace("\r", " ").Replace("\n", " ");
                    data = new Regex("\\s{2,50}", RegexOptions.IgnoreCase).Replace(data, " ");

                    var tName = new Regex("<title>(?<title>.+?)</title>").Match(data).Groups["title"].Value.Trim();

                    var wrTableExp = new Regex("(?<rules><table class=\"propositions\".+?</table>)", RegexOptions.Singleline);
                    var rules = wrTableExp.Match(data).Groups["rules"].Value;

                    if (string.IsNullOrWhiteSpace(rules))
                    {
                        wrTableExp = new Regex("Formal Propositions:</a></p>.+?(?<rules><table.+?</table>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        rules = wrTableExp.Match(data).Groups["rules"].Value;
                    }

                    if(string.IsNullOrWhiteSpace(rules))
                        continue;

                    var tRules = new TypeRules
                    {
                        Type = tName,
                        WhereRules = new List<WhereRule>()
                    };
                    sRules.TypeRulesSet.Add(tRules);

                    var rowsMatches = new Regex("<tr.*?>(?<row>.+?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(rules);
                    foreach (Match rowMatch in rowsMatches)
                    {
                        var row = rowMatch.Groups["row"].Value;
                        if(string.IsNullOrWhiteSpace(row))
                            continue;
                        var cellMatches = new Regex("<td.*?>(?<cell>.*?)</td>").Matches(row);
                        if (cellMatches.Count < 2)
                            continue;

                        var name = cellMatches.Cast<Match>().First().Groups["cell"].Value.Trim();
                        var description = cellMatches.Cast<Match>().Last().Groups["cell"].Value.Trim();

                        //Insert new lines instead of block HTML elements (<ol.*?>|<ul.*?>)
                        description = (new Regex(
                            "(<p.*?>|<div.*?>|<blockquote.*?>|<h.*?>|<dd.*?>|<dt.*?>|<hr.*?>|<pre.*?>)",
                            RegexOptions.IgnoreCase))
                            .Replace(description, "\n\n");

                        //keep lists
                        var listExpr = new Regex("<li.*?>", RegexOptions.IgnoreCase);
                        description = listExpr.Replace(description, "\n• ");

                        //replace HTML tags to create simple pure text
                        var tagExp = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
                        description = tagExp.Replace(description, "");

                        //remove Figure xxx - ...
                        description = (new Regex("Figure\\s*[0-9]+.*", RegexOptions.IgnoreCase)).Replace(
                            description, "");

                        //remove HISTORY 
                        description = (new Regex("HISTORY.*", RegexOptions.IgnoreCase)).Replace(description, "");
                        description = (new Regex("IFC.*?CHANGE.*")).Replace(description, "");

                        //replace HTML entities to create simple pure text
                        description = HttpUtility.HtmlDecode(description);

                        ////replace < and >
                        //description = description
                        //    .Replace(">", "&gt;")
                        //    .Replace("<", "&lt;");

                        tRules.WhereRules.Add(new WhereRule
                        {
                            Name = name,
                            Description = description.Trim(),
                            Definition = GetDefinition(expressData, tName, name)
                        });
                    }

                }
            }

            var serializer = new XmlSerializer(typeof(SchemaRules));
            using (var w = File.CreateText("..\\..\\..\\..\\XbimExpress\\XbimValidationGenerator\\Data\\" + schema + "_rules.xml"))
            {
                serializer.Serialize(w, sRules);
                w.Close();
            }
        }

        private static string GetDefinition(string data, string type, string rule)
        {
            var entExpr = new Regex("ENTITY\\s+?" + type + "(?<data>.+?)END_ENTITY;", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var tpExpr = new Regex("TYPE\\s+?" + type + "(?<data>.+?)END_TYPE;", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var def = entExpr.Match(data).Groups["data"].Value;
            if (string.IsNullOrWhiteSpace(def))
                def = tpExpr.Match(data).Groups["data"].Value;
            if(string.IsNullOrWhiteSpace(def))
                throw new Exception("Type data not found");

            var ruleExpr = new Regex("WHERE.+?" + rule + "\\s*?:\\s*?(?<rule>.+?);", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var ruleDef = ruleExpr.Match(def).Groups["rule"].Value;
            if (string.IsNullOrWhiteSpace(ruleDef))
                throw new Exception("Type data not found");
            return ruleDef;
        }
    }
}
