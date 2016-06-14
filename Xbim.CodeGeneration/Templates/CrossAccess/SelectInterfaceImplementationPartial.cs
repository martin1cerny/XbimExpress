using System.Collections.Generic;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.CrossAccess
{
    public partial class SelectInterfaceImplementation: ICodeTemplate
    {
        private readonly SelectType _source;
        private readonly SelectType _target;
        private NamedTypeHelper _helper;
        public GeneratorSettings Settings { get; set; }

        public SelectInterfaceImplementation(GeneratorSettings settings, SelectType source, SelectType target)
        {
            _source = source;
            _target = target;
            _helper = new NamedTypeHelper(source, settings);
            Settings = settings;
        }

        public string Name => _source.Name;

        private string OwnNamespace => _helper.FullNamespace;

        public string Namespace =>
            $"{Settings.Namespace}.{Settings.SchemaInterfacesNamespace}.{_target.ParentSchema.Name}";

        public string Inheritance => "";

        public IEnumerable<string> Using
        {
            get { yield break;}
        }
    }
}
