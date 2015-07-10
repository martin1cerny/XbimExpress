using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Xbim.Gppg;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Xbim.ExpressParser
{
    internal partial class Parser
    {
        Scanner _scanner;
        public TextWriter Output { get; set; }

        private Schemas.Schema _schema;
        private string _baseNamespaceName;
        private Dictionary<string, IEnumerable<string>> _selectInterfaces = new Dictionary<string,IEnumerable<string>>();
        private string _targetDir;
        private Dictionary<string, CodeNamespace> _types = new Dictionary<string, CodeNamespace>();
        private IfcVersionEnum _version;

        internal Parser(Scanner lex, IfcVersionEnum version): base(lex)
        {
            _scanner = lex;
            _version = version;

            //cache existing types in IFC2x3 assembly
            var assembly = Assembly.GetAssembly(typeof(Xbim.Ifc2x3.MeasureResource.IfcLabel));
            var types = assembly.GetTypes().Where(t => t.Name.StartsWith("Ifc")).ToList();
            assembly = Assembly.GetAssembly(typeof(Xbim.XbimExtensions.SelectTypes.IfcActorSelect));
            var commonTypes = assembly.GetTypes().Where(t => t.Name.StartsWith("Ifc"));
            types.AddRange(commonTypes);
            _ifcTypes = types;
            
            switch (version)
            {
                case IfcVersionEnum.IFC2x3:
                    _schema = Schemas.Schema.LoadIFC2x3();
                    _baseNamespaceName = "Xbim.Ifc2x3";
                    _targetDir = Path.Combine(Directory.GetCurrentDirectory(), "Ifc2x3");
                    break;
                case IfcVersionEnum.IFC4:
                    _schema = Schemas.Schema.LoadIFC4();
                    _baseNamespaceName = "Xbim.Ifc4";
                    _targetDir = Path.Combine(Directory.GetCurrentDirectory(), "Ifc4");
                    break;
                default:
                    break;
            }

            if (!Directory.Exists(_targetDir))
                Directory.CreateDirectory(_targetDir);
        }


        private List<Type> _ifcTypes;


        private Dictionary<string, string> _missingTypes = new Dictionary<string,string>();
        private void CheckTypeExists(string typeName)
        {
            var searchName = typeName;
            if (searchName.EndsWith("Enum"))
                searchName = typeName.Substring(0, typeName.Length - 4);
            if (!_ifcTypes.Any(t => t.Name == searchName || t.Name == typeName))
            {
                var domain = _schema.GetDomainForType(typeName);
                var msg = String.Format("{0}/{1} (line {2})", domain.Name, typeName, _scanner.yylloc.StartLine);
                _missingTypes.Add(typeName, msg);
            }
        }


        private void CreateEnumeration(string name, IEnumerable<string> members)
        {
            if (_ifcTypes.Any(t => t.Name == name))
                return;

            var ns = GetOrCreateNamespaceForEnum(name);
            var enu = new CodeTypeDeclaration(name) { IsEnum = true};
            ns.Types.Add(enu);

            //add all members
            foreach (var item in members)
            {
                enu.Members.Add(new CodeMemberField() { Name = item});
            }
        }

        private CodeNamespace GetOrCreateNamespaceForType(string name)
        {
            var domain = _schema.GetDomainForType(name);
            if (domain == null)
                throw new ArgumentException(name + " is not defined in the structure of the schema", "name");

            var nsName = String.Format("{0}.{1}", _baseNamespaceName, domain.Name);

            CodeNamespace ns = null;
            if (_types.TryGetValue(name, out ns))
                return ns;
            
            ns = new CodeNamespace(nsName);
            ns.Imports.Add(new CodeNamespaceImport("Xbim.XbimExtensions.SelectTypes"));
            ns.Imports.Add(new CodeNamespaceImport("System"));
            _types.Add(name, ns);

            return ns;
        }

        private CodeNamespace GetOrCreateNamespaceForEnum(string name)
        {
            var domain = _schema.GetDomainForType(name);
            if (domain == null)
                throw new ArgumentException(name + " is not defined in the structure of the schema", "name");

            var nsName = String.Format("{0}.{1}", _baseNamespaceName, domain.Name);

            CodeNamespace ns = null;
            if (_types.TryGetValue(name, out ns))
                return ns;

            ns = new CodeNamespace(nsName);
            _types.Add(name, ns);

            return ns;
        }

        private void CreateSelectInterface(string name, IEnumerable<string> subTypes)
        {
            _selectInterfaces.Add(name, subTypes);

            var ancestors = GetSelectAncestors(name);
            var decl = new CodeTypeDeclaration(name) { IsInterface = true};
            decl.BaseTypes.Add("ExpressSelectType");
            decl.BaseTypes.Add("IPersistIfcEntity");
            decl.BaseTypes.Add("ISupportChangeNotification");
            foreach (var ancestor in ancestors)
                decl.BaseTypes.Add(ancestor);

            var ns = new CodeNamespace("Xbim.XbimExtensions.SelectTypes");
            ns.Imports.Add(new CodeNamespaceImport("Xbim.XbimExtensions.Interfaces"));
            ns.Types.Add(decl);

            _types.Add(name, ns);

            //if there is any type which should reference this interface it should be updated
            UpdateInheritanceOfSelectType(name);
        }

        private IEnumerable<string> GetSelectAncestors(string type)
        {
            foreach (var pair in _selectInterfaces)
            {
                if (pair.Value.Contains(type))
                    yield return pair.Key;
            }
        }

        private void UpdateInheritanceOfSelectType(string selectTypeName)
        { 
            var types = _selectInterfaces[selectTypeName];

            //find the existing types
            foreach (var ns in _types.Values)
            {
                foreach (var type in ns.Types.Cast<CodeTypeDeclaration>())
                {
                    if (types.Contains(type.Name))
                    {
                        var baseTypes = type.BaseTypes.Cast<CodeTypeReference>();
                        if (!baseTypes.Any(bt => bt.BaseType == selectTypeName))
                        {
                            type.BaseTypes.Add(selectTypeName);
                        }
                    }
                }
            }
        }

        private void Finished()
        {
            //clear last results
            ClearOutputSpace();

            // Generate the code with the C# code provider.
            CSharpCodeProvider provider = new CSharpCodeProvider();
            var options = new System.CodeDom.Compiler.CodeGeneratorOptions()
            {
                IndentString = "    ",
                VerbatimOrder = true,
                BracingStyle = "C"
            };

            //save result to the files and directories
            foreach (var pair in _types)
            {
                var ns = pair.Value;
                var type = pair.Key;

                //skip types which are implemented already
                if (!_missingTypes.ContainsKey(type))
                    continue;

                //set up output dir
                var dirName = ns.Name.Split('.').Last();
                dirName = Path.Combine(_targetDir, dirName);
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                var file = Path.Combine(dirName, type + ".cs");
                using (var writer = new StreamWriter(file))
                {
                    var codeWriter = new IndentedTextWriter(writer, "    ");
                    provider.GenerateCodeFromNamespace(ns, codeWriter, options);
                    codeWriter.Close();
                }
            }


            if (Output == null)
                Output = File.CreateText("output.txt");
            foreach (var missingType in _missingTypes.Keys)
            {
                var ancestors = GetSelectAncestors(missingType);
                var ancMsg = String.Join(", ", ancestors);

                IEnumerable<string> selectDescendants = null;
                if (_selectInterfaces.TryGetValue(missingType, out selectDescendants))
                {
                    var descendants = String.Join(", ", selectDescendants);
                    Output.WriteLine("{0} ({1}), Is implemented in: {2}", _missingTypes[missingType], ancMsg, descendants); 
                }
                else
                    Output.WriteLine("{0} ({1})", _missingTypes[missingType], ancMsg);
            }
        }


        private void ClearOutputSpace()
        {
            foreach (var file in Directory.EnumerateFiles(_targetDir, "*.*", SearchOption.AllDirectories))
                File.Delete(file);

            foreach (var dir in Directory.EnumerateDirectories(_targetDir))
                Directory.Delete(dir);
        }
    }

    public enum IfcVersionEnum
    {
        IFC2x3,
        IFC4
    }
}
