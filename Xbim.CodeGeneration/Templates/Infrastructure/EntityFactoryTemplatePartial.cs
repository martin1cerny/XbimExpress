using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class EntityFactoryTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;
        private readonly SchemaDefinition _schema;

        public EntityFactoryTemplate(GeneratorSettings settings, SchemaDefinition schema)
        {
            _settings = settings;
            _schema = schema;
        }

        public string Name
        {
            get { return $"{_settings.EntityFactory}_{_schema.Identification}" ; }
        }

        public string Namespace
        {
            get { return _settings.Namespace; }
        }

        public string Inheritance
        {
            get { return _settings.IsInfrastructureSeparate ? ": " + _settings.EntityFactoryInterface : ""; }
        }

        public IEnumerable<string> Using
        {
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();

                namedOccurances.AddRange(NonAbstractEntities);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, _settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                if(_settings.IsInfrastructureSeparate)
                    result.Add(_settings.InfrastructureNamespace);

                return result;
                
            }
        }

        private string InstantiableEntityInterface  => _settings.InstantiableEntityInterface; 
        private string ModelInterface  => _settings.ModelInterface; 

        private IEnumerable<EntityDefinition> NonAbstractEntities  => _schema.Entities.Where(d => d.Instantiable);

        private IEnumerable<DefinedType> DefinedTypes => _schema.Types; 

        private IEnumerable<string> SchemasIds  => new[] { _schema.Identification }; 
    }
}
