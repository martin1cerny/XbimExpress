using System.Collections.Generic;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Settings
{
    public class GeneratorSettings
    {
        /// <summary>
        /// General namespace for all model classes, enumerations and structures. It is only used internally and
        /// it's value is set in the Generator class.
        /// </summary>
        internal string Namespace { get; set; }

        /// <summary>
        /// General namespace for all infrastructure interfaces. It is only used internally and
        /// it's value is set in the Generator class.
        /// </summary>
        internal string InfrastructureNamespace { get; set; }


        public GeneratorSettings()
        {
            //set defaults
            OutputPath = "";
            ModelInterface = "IModel";
            EntityCollentionInterface = "IEntityCollection";
            PersistEntityInterface = "IPersistEntity";
            PersistInterface = "IPersist";
            TransactionInterface = "ITransaction";
            ItemSetClassName = "ItemSet";
            EntityFactory = "EntityFactory";
            EntityFactoryInterface = "IEntityFactory";
            InstantiableEntityInterface = "IInstantiableEntity";
            SchemaInterfacesNamespace = "Interfaces";
        }


        /// <summary>
        /// List of derived attributes to be left out. Derived attributes are kind of a mess in IFC express 
        /// schema. They are sometimes assumed to exist on a higher level or on the level of select types which is
        /// impossible in Express. So manual changes might need to be done. This is then to preserve redefinition
        /// of such attributes.
        /// </summary>
        public List<AttributeInfo> IgnoreDerivedAttributes { get; set; }

        /// <summary>
        /// Project to be used in 'GenerateCrossAccess(GeneratorSettings settings, SchemaModel schema, SchemaModel remoteSchema)'
        /// for a remote schema. This has an influence on namespaces and references
        /// </summary>
        public string CrossAccessProjectPath { get; set; }
        internal string CrossAccessNamespace { get; set; }

        /// <summary>
        /// If GenerateAllAsInterfaces is true this namespace will contain all entity interfaces
        /// </summary>
        public string SchemaInterfacesNamespace { get; set; }

        /// <summary>
        /// Interface used to label entities which can be instantiated (are non-abstract)
        /// </summary>
        public string InstantiableEntityInterface { get; set; }

        /// <summary>
        /// Name of the static class which is the only point where you can insnantiate new 
        /// entities. It is designed to be used from model or its entity collection
        /// </summary>
        public string EntityFactory { get; set; }

        /// <summary>
        /// Name of the entity factory interface. It is possible to write general models and 
        /// serializers/deserializers using this interface alongside with the others.
        /// </summary>
        public string EntityFactoryInterface { get; set; }

        /// <summary>
        /// Name of the class which will be used for all lists, sets and arrays in the generated code.
        /// This class has only internal constructor and is integral part of the framework.
        /// All attributes which hold this class are initiated by default so you just add objects to it.
        /// It supports transactions so if your model is transactional it will use transactions
        /// when items are added or removed.
        /// </summary>
        public string ItemSetClassName { get; set; }

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

        /// <summary>
        /// All entities implement this interface so you can use it for the most general search
        /// accross the model.
        /// </summary>
        public string PersistEntityInterface { get; set; }

        /// <summary>
        /// All persistent data items implement this interface.
        /// </summary>
        public string PersistInterface { get; set; }

        /// <summary>
        /// Interface for the basic transactions. All setters and collection operations
        /// are transactional if the model is transactional (there is a flag in the interface).
        /// You will have to implement this class as a part of your infrastructure.
        /// </summary>
        public string TransactionInterface { get; set; }

        /// <summary>
        /// Optional structure used by the generator to split the code into
        /// multiple namespaces. Resulting files will be located in the corresponding
        /// folders.
        /// </summary>
        public DomainStructure Structure { get; set; }

        /// <summary>
        /// Optional structure used to identify the right namespaces for
        /// cross access interfaces implementations
        /// </summary>
        public DomainStructure CrossAccessStructure { get; set; }

        internal IEnumerable<string> SchemasIds { get; set; }
    }

    public class AttributeInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string EntityName { get; set; }
    }
}
