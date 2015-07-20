using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Settings
{
    public class GeneratorSettings
    {
        public GeneratorSettings()
        {
        //set defaults
            Namespace = "GeneratedCode";
            OutputPath = "";

            ClassSettings = new EntitySettings();
            TypeSettings = new DefinedTypeSettings();
            SelectSettings = new SelectTypeSettings();
        }

        public string OutputPath { get; set; }

        public string Namespace { get; set; }

        public DomainStructure Structure { get; set; }

        public EntitySettings ClassSettings { get; set; }

        public DefinedTypeSettings TypeSettings { get; set; }
        public SelectTypeSettings SelectSettings { get; set; }
    }
}
