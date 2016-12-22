﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Xbim.ExpressParser;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using XbimTranslatorGenerator.Differences;
using XbimTranslatorGenerator.Templates;

namespace XbimTranslatorGenerator
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            const string prjPath = "..\\..\\..\\..\\XbimEssentials\\Xbim.SchemaTranslation";
            var ifc2X3 = GetSchema(Schemas.IFC2X3_TC1, SchemaSources.IFC2x3_TC1);
            var ifc4 = GetSchema(Schemas.IFC4_ADD1, SchemaSources.IFC4_ADD1);
            Console.WriteLine(@"Generation of the code for translation from {0} to {1}", ifc2X3.FirstSchema.Name, ifc4.FirstSchema.Name);

            var project = GetProject(prjPath);
            Settings.Namespace = GetNamespace(project);

            var w = new Stopwatch();
            w.Start();

            //create diff model and only take these which don't have an exact match
            var entityMatches = EntityDefinitionMatch.GetMatches(ifc2X3, ifc4).Where(em => em.MatchType != EntityMatchType.Identity).ToList();

            //create interface
            ProcessTemplate(new ITranslatorTemplate(), project);
            var entityCount = entityMatches.Count;

            Console.WriteLine(@"{0} entities to process.", entityCount);
            
            //create translators from templates
            foreach (var entityMatch in entityMatches)
            {
                var template = new TranslatorTemplate(entityMatch);
                //write uniform header for all generated files
                ProcessTemplate(template, project);
            }

            Console.WriteLine();
            w.Stop();
            Console.WriteLine(@"Processing time: {0} ms", w.ElapsedMilliseconds);
            project.Save();
        }

        private static void ProcessTemplate(ICodeTemplate template, ProjectRootElement project)
        {
            var rootNamespace = GetNamespace(project);
            var localNamespace = template.Namespace.Substring(rootNamespace.Length);

            var fileName = template.Name + ".cs";
            var localPath = Path.Combine(localNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
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
                    g => g.Children.FirstOrDefault(e => ((ProjectPropertyElement)e).Name == "RootNamespace"))
                    .FirstOrDefault(e => e != null) as ProjectPropertyElement;
            return element != null ? element.Value : Path.GetFileNameWithoutExtension(project.FullPath);
        }

        private static void WriteHeader(ICodeTemplate template)
        {
            template.WriteLine("// ------------------------------------------------------------------------------");
            template.WriteLine("// <auto-generated>");
            template.WriteLine("//     This code was generated by a tool XbimTranslatorGenerator");
            //this causes source control to pick up ALL the files because they got changed even if they are de-facto the same
            //template.WriteLine("//		{0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            template.WriteLine("//  ");
            template.WriteLine("//     Changes to this file may cause incorrect behaviour and will be lost if");
            template.WriteLine("//     the code is regenerated.");
            template.WriteLine("// </auto-generated>");
            template.WriteLine("// ------------------------------------------------------------------------------");
        }
        private static SchemaModel GetSchema(string data, string source)
        {
            var parser = new ExpressParser();
            var result = parser.Parse(data, source);
            if (!result)
                throw new Exception("Error parsing schema file");
            return parser.SchemaInstance;
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
