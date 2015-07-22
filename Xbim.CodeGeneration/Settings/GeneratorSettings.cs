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
        internal string Namespace { get; set; }
        public GeneratorSettings()
        {
            //set defaults
            OutputPath = "";
            ModelInterface = "IModel";
            EntityCollentionInterface = "IEntityCollection";
            PersistEntityInterface = "IPersistEntity";
            TransactionInterface = "ITransaction";
        }

        /// <summary>
        /// Output directory for the resulting files. If it doesn't exist it
        /// gets created. This is also used as the global namespace for all
        /// the generated content.
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Model is the object that holds all the entities. Every entity also referes to this
        /// object to be able to change values under transaction and to be able to show any
        /// inverse properties from the model. Model is also responsible for any kind of serialization
        /// or deserialization. It is only defined as an interface in this tool. It's implementation
        /// is up to you. We have a generic model implementation as a part of our xBIM project.
        /// <br />
        /// Default value: IModel
        /// </summary>
        public string ModelInterface { get; set; }

        /// <summary>
        /// This collection will reside inside of the model as a dictionary holding all entity instances
        /// <br />
        /// Default value: IEntityCollection
        /// </summary>
        public string EntityCollentionInterface { get; set; }

        public string PersistEntityInterface { get; set; }
        public string TransactionInterface { get; set; }

        /// <summary>
        /// Optional structure used by the generator to split the code into
        /// multiple namespaces. Resulting files will be located in the corresponding
        /// folders.
        /// </summary>
        public DomainStructure Structure { get; set; }

    }
}
