using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Differences;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.CrossInstantiation
{
    public partial class CreatorTemplate: ICodeTemplate
    {
        public GeneratorSettings Settings { get; private set; }
        public List<EntityDefinitionMatch> Matches { get; private set; }

        public string Name => "Create";

        public string Namespace => "Xbim.Ifc2x3.Interfaces.IFC4";

        public string Inheritance => null;

        public IEnumerable<string> Using => Enumerable.Empty<string>();

        public CreatorTemplate(GeneratorSettings settings, List<EntityDefinitionMatch> matches)
        {
            Settings = settings;
            Matches = matches;
        }

        private string GetFullName(EntityDefinition entity)
        {
            if (string.Equals(entity.SchemaModel.FirstSchema.Name, "IFC2X3"))
            {
                var schema = "Ifc2x3";
                var ns = Settings.Structure.GetDomainForType(entity.Name).Name;
                return $"{schema}.{ns}.{entity.Name}";
            }
            else
            {
                var ns = Settings.CrossAccessStructure.GetDomainForType(entity.Name).Name;
                return $"{ns}.{entity.Name}";
            }
        }
    }
}
