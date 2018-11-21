using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
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
            var modelPrjPath = GetDirectory(settings.OutputPath);
            var targetPrjPath = GetDirectory(settings.CrossAccessProjectPath);

            //set the right target namespace for usings
            settings.Namespace = settings.OutputPath;
            if (!settings.Namespace.StartsWith("Xbim."))
                settings.Namespace = "Xbim." + settings.Namespace;
            settings.CrossAccessNamespace = settings.Namespace + "." + settings.SchemaInterfacesNamespace;

            var entityMatches = EntityDefinitionMatch.GetMatches(schema, remoteSchema).ToList();

            var templates =
                entityMatches.Where(m => m.Target != null)
                    .Select(m => new EntityInterfaceImplementation(settings, m, entityMatches) as ICodeTemplate);
            var selectTemplates =
                GetSelectsToImplement(schema, remoteSchema, entityMatches)
                    .Select(s => new SelectInterfaceImplementation(settings, s.Item1, s.Item2));
            var infrastructureTemplates = new ICodeTemplate[] { new CreatorTemplate(settings, entityMatches) };

            var toProcess = templates.Concat(selectTemplates).Concat(infrastructureTemplates);

            //toProcess.ToList().ForEach(t => ProcessTemplate(t, modelProject));
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
            //set schema IDs for this generation session
            settings.SchemasIds = schema.Schemas.Select(s => s.Identification);

            //set namespaces
            settings.Namespace = settings.OutputPath;
            if (!settings.Namespace.StartsWith("Xbim."))
                settings.Namespace = "Xbim." + settings.Namespace;
            settings.InfrastructureNamespace = @"Xbim.Common";

            var modelTemplates = new List<ICodeTemplate>();
            modelTemplates.AddRange(
                schema.Get<DefinedType>().Select(type => new DefinedTypeTemplate(settings, type)));
            modelTemplates.AddRange(schema.Get<SelectType>().Select(type => new SelectTypeTemplate(settings, type)));
            modelTemplates.AddRange(schema.Get<EntityDefinition>().Select(type => new EntityInterfaceTemplate(settings, type)));
            modelTemplates.AddRange(
                schema.Get<EnumerationType>().Select(type => new EnumerationTemplate(settings, type)));

            // entity factory for this schema and any extensions
            modelTemplates.AddRange(
                schema.Schemas.Select(s => new EntityFactoryTemplate(settings, s)));
            
            //inner model infrastructure
            modelTemplates.Add(new ItemSetTemplate(settings));
            modelTemplates.Add(new OptionalItemSetTemplate(settings));

            //modelTemplates.ForEach(t => ProcessTemplate(t, settings.Namespace));
            Parallel.ForEach(modelTemplates, tmpl => ProcessTemplate(tmpl, settings.Namespace));
            
            return true;
        }

        private static void ReferenceProject(ProjectRootElement referenced, ProjectRootElement referencing)
        {
            //get project references 
            const string itemType = "ProjectReference";
            var references = referencing.ItemGroups.FirstOrDefault(g => g.Items.All(i => i.ItemType == itemType)) ??
                             referencing.AddItemGroup();

            var referencedPath = new Uri(referenced.FullPath, UriKind.Absolute);
            var referencingPathString = (Path.GetDirectoryName(referencing.FullPath) ?? "") +
                                        Path.DirectorySeparatorChar;
            var referencingPath = new Uri(referencingPathString, UriKind.Absolute);

            var relPath = referencingPath.MakeRelativeUri(referencedPath).ToString();

            //check if it is not there already
            if (references.Children.Any(c =>
            {
                var itemElement = c as ProjectItemElement;
                return itemElement != null && itemElement.Include == relPath;
            })) return;

            references.AddItem(itemType, relPath, new[]
            {
                new KeyValuePair<string, string>("Project", GetProjectId(referenced)),
                new KeyValuePair<string, string>("Name", Path.GetFileNameWithoutExtension(referenced.FullPath))
            });
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

        private static string GetNamespace(ProjectRootElement project)
        {
            var element =
                project.PropertyGroups.Select(
                    g => g.Children.FirstOrDefault(e => ((ProjectPropertyElement) e).Name == "RootNamespace"))
                    .FirstOrDefault(e => e != null) as ProjectPropertyElement;
            return element != null ? element.Value : Path.GetFileNameWithoutExtension(project.FullPath);
        }

        private static string GetProjectId(ProjectRootElement project)
        {
            var element =
                project.PropertyGroups.Select(
                    g => g.Children.FirstOrDefault(e => ((ProjectPropertyElement) e).Name == "ProjectGuid"))
                    .FirstOrDefault(e => e != null) as ProjectPropertyElement;
            return element != null ? element.Value : "";
        }
    }
}