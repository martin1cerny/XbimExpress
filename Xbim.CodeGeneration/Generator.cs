using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Xbim.CodeGeneration.Settings;
using Xbim.CodeGeneration.Templates;
using Xbim.CodeGeneration.Templates.Infrastructure;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration
{
    public class Generator
    {
        public static bool Generate(GeneratorSettings settings, SchemaModel schema)
        {
            //get output paths
            var modelPrjPath = GetDirectory(settings.OutputPath);
            var infraPrjPath = settings.IsInfrastructureSeparate
                ? GetDirectory(settings.InfrastructureOutputPath)
                : modelPrjPath;

            //get or create target CS projects
            var modelProject = GetProject(modelPrjPath);
            var infraProject = settings.IsInfrastructureSeparate ? GetProject(infraPrjPath) : modelProject;

            //set namespaces
            settings.Namespace = GetNamespace(modelProject);
            settings.InfrastructureNamespace = settings.IsInfrastructureSeparate ? GetNamespace(infraProject) : settings.Namespace;

            var modelTemplates = new List<ICodeTemplate>();
            modelTemplates.AddRange(
                schema.Get<DefinedType>().Select(type => new DefinedTypeTemplate(settings, type)));
            modelTemplates.AddRange(schema.Get<SelectType>().Select(type => new SelectTypeTemplate(settings, type)));
            modelTemplates.AddRange(
                schema.Get<EntityDefinition>().Select(type => new EntityTemplate(settings, type)));
            modelTemplates.AddRange(
                schema.Get<EnumerationType>().Select(type => new EnumerationTemplate(settings, type)));
            modelTemplates
                .Add(new EntityFactoryTemplate(settings, schema));
            modelTemplates
                .Add(new ItemSetTemplate(settings));
            foreach (var tmpl in modelTemplates)
                ProcessTemplate(tmpl, modelProject);


            var infrastructureTemplates = new List<ICodeTemplate>
            {
                new PersistEntityTemplate(settings),
                new ModelTemplate(settings),
                new EntityCollectionTemplate(settings),
                new TransactionTemplate(settings),
                new EntityFactoryInterfaceTemplate(settings)
            };
            foreach (var template in infrastructureTemplates)
                ProcessTemplate(template, infraProject);


            //make sure model project references infrastructural project
            if (modelProject != infraProject)
            {
                ReferenceProject(infraProject, modelProject);
            }

            //save changes to the projects
            modelProject.Save();
            infraProject.Save();

            return true;
        }

        private static void ReferenceProject(ProjectRootElement referenced, ProjectRootElement referencing)
        {
            //get project references 
            const string itemType = "ProjectReference";
            var references = referencing.ItemGroups.FirstOrDefault(g => g.Items.All(i => i.ItemType == itemType)) ??
                           referencing.AddItemGroup();

            var referencedPath = new Uri(referenced.FullPath, UriKind.Absolute);
            var referencingPath = new Uri(Path.GetDirectoryName(referencing.FullPath) ?? "", UriKind.Absolute);

            var relPath = referencingPath.MakeRelativeUri(referencedPath).ToString();

            //check if it is not there already
            if (references.Children.Any(c =>
            {
                var itemElement = c as ProjectItemElement;
                return itemElement != null && itemElement.Include == relPath;
            })) return;

            references.AddItem(itemType, relPath, new []
            {
                new KeyValuePair<string, string>("Project", GetProjectId(referenced)), 
                new KeyValuePair<string, string>("Name", Path.GetFileNameWithoutExtension(referenced.FullPath)) 
            });
        }

        private static ProjectRootElement GetProject(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            var suggestedName = new DirectoryInfo(directoryPath).Name;

            var projFile =
                Directory.EnumerateFiles(directoryPath, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            return projFile != null ? OpenProject(projFile) : InitProject(suggestedName, directoryPath);
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
            template.WriteLine("//		{0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            template.WriteLine("//  ");
            template.WriteLine("//     Changes to this file may cause incorrect behaviour and will be lost if");
            template.WriteLine("//     the code is regenerated.");
            template.WriteLine("// </auto-generated>");
            template.WriteLine("// ------------------------------------------------------------------------------");
        }

        private static void ProcessTemplate(ICodeTemplate template, ProjectRootElement project)
        {
            var rootNamespace = GetNamespace(project);
            var localNamespace = template.Namespace.Substring(rootNamespace.Length);

            var fileName = template.Name + ".cs";
            var localPath = Path.Combine(localNamespace.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries));
            var projectPath = Path.GetDirectoryName(project.FullPath) ?? "";
            var fullPath = Path.Combine(projectPath, localPath);

            if (!Directory.Exists(fullPath) && !String.IsNullOrWhiteSpace(fullPath))
                Directory.CreateDirectory(fullPath);

            var filePath = Path.Combine(fullPath, fileName);

            using (var file = File.CreateText(filePath))
            {
                WriteHeader(template);
                var code = template.TransformText();
                file.Write(code);
                file.Close();
            }

            var projectFilePath = Path.Combine(localPath, fileName);
            AddCompilationItem(projectFilePath, project);
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
                    g => g.Children.FirstOrDefault(e => ((ProjectPropertyElement)e).Name == "ProjectGuid"))
                    .FirstOrDefault(e => e != null) as ProjectPropertyElement;
            return element != null ? element.Value : "";
        }

        private static ProjectRootElement InitProject(string name, string directory)
        {
            var tmpl = new CSProjectTemplate(name);
            var data = tmpl.TransformText();
            var fileName = Path.Combine(directory, name + ".csproj");

            using (var w = File.CreateText(fileName))
            {
                w.Write(data);
                w.Close();
            }

            return OpenProject(fileName);
        }

        private static ProjectRootElement OpenProject(string path)
        {
            var projElement = ProjectRootElement.Open(path);
            if (projElement == null)
                throw new Exception("Failed to open existing CS project: " + path);

            return projElement;
        }

        private static void AddCompilationItem(string item, ProjectRootElement project)
        {
            const string itemType = "Compile";
            var includes = project.ItemGroups.FirstOrDefault(g => g.Items.All(i => i.ItemType == itemType)) ??
                           project.AddItemGroup();

            //check if it is not there already
            if (includes.Children.Any(c =>
            {
                var itemElement = c as ProjectItemElement;
                return itemElement != null && itemElement.Include == item;
            })) return;

            includes.AddItem(itemType, item);
        }
    }
}