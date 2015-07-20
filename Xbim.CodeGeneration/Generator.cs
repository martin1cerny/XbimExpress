using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Xbim.CodeGeneration.Settings;
using Xbim.CodeGeneration.Templates;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration
{
    public class Generator
    {
        public GeneratorSettings Settings { get; private set; }
        public SchemaModel SchemaModel { get; private set; }

        private ProjectItemGroupElement _includeItemGroupElement;
        private bool _newProject;

        public Generator(GeneratorSettings settings, SchemaModel schema)
        {
            Settings = settings;
            SchemaModel = schema;
        }

        public bool Generate()
        {
            var path = Settings.OutputPath;
            if (Directory.Exists(path))
            {
                var projFile = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if(projFile != null)
                    OpenProject(projFile);
                else
                    InitProject();
            }
            else
                InitProject();


            foreach (var type in SchemaModel.Get<DefinedType>())
            {
                var tmpl = new DefinedTypeTemplate(Settings, type);
                var code = tmpl.TransformText();

                //save file
                SaveFile(code, type.Name, tmpl.Namespace);
            }

            foreach (var type in SchemaModel.Get<SelectType>())
            {
                var tmpl = new SelectTypeTemplate(Settings, type);
                var code = tmpl.TransformText();

                //save file
                SaveFile(code, type.Name, tmpl.Namespace);
            }

            //save project file
            if (_newProject)
            {
                var projPath = Path.Combine(path, SchemaModel.Schema.Name + ".csproj");
                _includeItemGroupElement.ContainingProject.Save(projPath);
            }
            else
                _includeItemGroupElement.ContainingProject.Save();

            return true;
        }

        private void SaveFile(string code, string codeName, string codeNamespace)
        {
            var path = Path.Combine(codeNamespace.Split('.'));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, codeName + ".cs");

            using (var file = File.CreateText(path))
            {
                file.Write(code);
                file.Close();

                //add file to the project if it is not there already
                AddCompilationItem(path);
            }
            

        }

        private void InitProject()
        {
            var projElement = ProjectRootElement.Create();
            var group = projElement.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug");
            group.AddProperty("Platform", "AnyCPU");

            // references
            AddProjectItems(projElement, "Reference", "System", "System.Core");

            // items to compile
            _includeItemGroupElement = projElement.AddItemGroup();
            _newProject = true;
        }

        private void OpenProject(string path)
        {
            var projElement = ProjectRootElement.Open(path);

            if(projElement == null)
                throw new Exception("Failed to open existing CS project: " + path);

            // items to compile
            _includeItemGroupElement = projElement.ItemGroups.FirstOrDefault(g => g.Items.All(i => i.ItemType == "Compile")) ??
                                       projElement.AddItemGroup();
            _newProject = false;
        }

        private void AddCompilationItem(string item)
        {
                _includeItemGroupElement.AddItem("Compile", item);
        }

        private void AddProjectItems(ProjectRootElement elem, string groupName, params string[] items)
        {
            var group = elem.AddItemGroup();
            foreach (var item in items)
            {
                group.AddItem(groupName, item);
            }
        }
    }
}
