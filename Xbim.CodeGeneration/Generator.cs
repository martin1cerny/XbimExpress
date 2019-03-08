using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Differences;
using Xbim.CodeGeneration.Settings;
using Xbim.CodeGeneration.Templates;
using Xbim.CodeGeneration.Templates.CrossAccess;
using Xbim.CodeGeneration.Templates.CrossInstantiation;
using Xbim.CodeGeneration.Templates.Infrastructure;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration
{
    public class Generator
    {

        public static bool GenerateCrossAccess(GeneratorSettings settings, SchemaModel schema, SchemaModel remoteSchema)
        {
            //set the right target namespace for usings
            settings.Namespace = settings.OutputPath;
            if (!settings.Namespace.StartsWith("Xbim."))
                settings.Namespace = "Xbim." + settings.Namespace;
            settings.CrossAccessNamespace = settings.CrossAccessProjectPath + "." + settings.SchemaInterfacesNamespace;

            var entityMatches = EntityDefinitionMatch.GetMatches(schema, remoteSchema).ToList();

            var templates =
                entityMatches.Where(m => m.Target != null)
                    .Select(m => new EntityInterfaceImplementation(settings, m, entityMatches) as ICodeTemplate);
            var selectTemplates =
                GetSelectsToImplement(schema, remoteSchema, entityMatches)
                    .Select(s => new SelectInterfaceImplementation(settings, s.Item1, s.Item2));
            var infrastructureTemplates = new ICodeTemplate[] { new CreatorTemplate(settings, entityMatches) };

            var toProcess = templates.Concat(selectTemplates).Concat(infrastructureTemplates).ToList();

            //toProcess.ForEach(t => ProcessTemplate(t, settings.Namespace));
            Parallel.ForEach(toProcess, t => ProcessTemplate(t, settings.Namespace));

            return true;
        }

        private static IEnumerable<Tuple<SelectType, SelectType>> GetSelectsToImplement(SchemaModel schema, SchemaModel remote,
            IEnumerable<EntityDefinitionMatch> matches)
        {
            var definitionMatches = matches as IList<EntityDefinitionMatch> ?? matches.ToList();
            var targets = remote.Get<SelectType>().ToList();
            foreach (var source in schema.Get<SelectType>())
            {
                var target = targets.FirstOrDefault(t => t.Name == source.Name);
                if (target == null)
                    continue;
                if (EntityInterfaceImplementation.IsSelectCompatible(source, target, definitionMatches))
                    yield return new Tuple<SelectType, SelectType>(source, target);
            }
        }

        public static bool GenerateSchema(GeneratorSettings settings, SchemaModel schema)
        {
            if (!Directory.Exists(settings.OutputPath))
                Directory.CreateDirectory(settings.OutputPath);

            // make sure IDs are stable over regenerations
            SetTypeNumbers(schema, settings.OutputPath);

            //set schema IDs for this generation session
            settings.SchemasIds = schema.Schemas.Select(s => s.Identification);

            //set namespaces
            settings.Namespace = settings.OutputPath;
            if (!settings.Namespace.StartsWith("Xbim."))
                settings.Namespace = "Xbim." + settings.Namespace;
            settings.InfrastructureNamespace = @"Xbim.Common";

            var templates = new List<ICodeTemplate>();
            templates.AddRange(
                schema.Get<DefinedType>().Select(type => new DefinedTypeTemplate(settings, type)));
            templates.AddRange(schema.Get<SelectType>().Select(type => new SelectTypeTemplate(settings, type)));
            templates.AddRange(schema.Get<EntityDefinition>().Select(type => new EntityInterfaceTemplate(settings, type)));
            templates.AddRange(
                schema.Get<EnumerationType>().Select(type => new EnumerationTemplate(settings, type)));

            // entity factory for this schema and any extensions
            templates.AddRange(
                schema.Schemas.Select(s => new EntityFactoryTemplate(settings, s)));
            
            //inner model infrastructure
            templates.Add(new ItemSetTemplate(settings));
            templates.Add(new OptionalItemSetTemplate(settings));

            //templates.ForEach(t => ProcessTemplate(t, settings.Namespace));
            Parallel.ForEach(templates, tmpl => ProcessTemplate(tmpl, settings.Namespace));
            
            return true;
        }

        /// <summary>
        /// Use this method before you generate the source code to keep type IDs consistent even when you
        /// move the content in the EXPRESS file. When you rename an entity type in EXPRESS this will make
        /// sure that new Entity will have new ID unless you modify the ID file manually.
        /// </summary>
        /// <param name="model">Schema model</param>
        /// <param name="directory">Target directory</param>
        /// <returns></returns>
        private static int SetTypeNumbers(SchemaModel model, string directory)
        {
            var max = 0;
            var types = model.Get<NamedType>().ToList();
            const string extension = "_TYPE_IDS.csv";


            var ids = new Dictionary<string, int>();
            var file = model.FirstSchema.Name + extension;

            var source = Path.Combine(directory, file);
            if (File.Exists(source))
            {
                var data = File.ReadAllText(source);
                var kvps = data.Trim().Split('\n');
                foreach (var vals in kvps.Select(kvp => kvp.Split(',')))
                {
                    ids.Add(vals[0], int.Parse(vals[1]));
                }
            }

            //reset latest values
            foreach (var type in types.ToList())
            {
                if (!ids.TryGetValue(type.PersistanceName, out int id)) continue;
                type.TypeId = id;
                max = Math.Max(max, id);
                types.Remove(type);
            }

            //set new values to the new types
            foreach (var type in types)
                type.TypeId = ++max;

            using (var o = File.CreateText(source))
            {
                //save for the next processing
                foreach (var type in model.Get<NamedType>())
                {
                    o.Write("{0},{1}\n", type.PersistanceName, type.TypeId);
                }
                o.Close();
            }

            return max;
        }


        private static string GetDirectory(string suggestedPath)
        {
            var path = String.IsNullOrWhiteSpace(suggestedPath) ? Environment.CurrentDirectory : suggestedPath;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private static void WriteHeader(ICodeTemplate template)
        {
            template.WriteLine("// ------------------------------------------------------------------------------");
            template.WriteLine("// <auto-generated>");
            template.WriteLine("//     This code was generated by a tool Xbim.CodeGeneration ");
            //this causes source control to pick up ALL the files because they got changed even if they are de-facto the same
            //template.WriteLine("//		{0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            template.WriteLine("//  ");
            template.WriteLine("//     Changes to this file may cause incorrect behaviour and will be lost if");
            template.WriteLine("//     the code is regenerated.");
            template.WriteLine("// </auto-generated>");
            template.WriteLine("// ------------------------------------------------------------------------------");
        }

        private static readonly Regex CustomCodeRegex = new Regex("(//##).*?(//##)", RegexOptions.Singleline);

        private static Dictionary<string, string> GetSections(string content)
        {
            var result = new Dictionary<string,string>();
            var sections = CustomCodeRegex.Matches(content);
            foreach (Match section in sections)
            {
                var fli = section.Value.IndexOf('\n');
                var name = section.Value.Substring(0, fli).TrimStart('/', '#').Trim();
                result.Add(name, section.Value);
            }
            return result;
        }

        private static void ProcessTemplate(ICodeTemplate template, string rootNamespace)
        {
            var localNamespace = template.Namespace.Substring(rootNamespace.Length);

            var fileName = template.Name + ".cs";
            var localPath = Path.Combine(localNamespace.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries));
            var fullPath = Path.Combine(rootNamespace, localPath);

            if (!Directory.Exists(fullPath) && !String.IsNullOrWhiteSpace(fullPath))
                Directory.CreateDirectory(fullPath);

            var filePath = Path.Combine(fullPath, fileName);

            WriteHeader(template);
            var code = template.TransformText();
            //it is possible to keep in custom code if there are predefined slots for it
            if (code.Contains("//##") && File.Exists(filePath))
            {
                var oldFile = File.ReadAllText(filePath);
                var oldSections = GetSections(oldFile);
                var newSections = GetSections(code);

                foreach (var section in newSections)
                {
                    var name = section.Key;
                    string value;
                    if (oldSections.TryGetValue(name, out value))
                        code = code.Replace(section.Value, value);
                }
            }

            using (var file = File.CreateText(filePath))
            {
                file.Write(code);
                file.Close();
            }
        }
    }
}