using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class EntityFactoryTemplate : ICodeTemplate
    {
        private readonly GeneratorSettings _settings;
        private readonly SchemaDefinition _schema;

        public EntityFactoryTemplate(GeneratorSettings settings, SchemaDefinition schema)
        {
            _settings = settings;
            _schema = schema;

            var id = schema.Identification;
            Name = settings.EntityFactory + id[0].ToString().ToUpperInvariant() + id.Substring(1).ToLowerInvariant();
            var nameChars = Name.ToCharArray().ToList();
            while (nameChars.Contains('_'))
            {
                var idx = nameChars.IndexOf('_');
                nameChars.RemoveAt(idx);
                nameChars[idx] = nameChars[idx].ToString().ToUpperInvariant()[0];
            }
            Name = new string(nameChars.ToArray());
        }

        public string Name { get; }

        public string Namespace
        {
            get { return _settings.Namespace; }
        }

        public string Inheritance
        {
            get { return ": " + _settings.EntityFactoryInterface; }
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

                result.Add(_settings.InfrastructureNamespace);

                return result;

            }
        }

        private bool HasReferences => _schema.ReferencesSchemas.Any();

        private IEnumerable<string> ReferencedSchemas => _schema.ReferencesSchemas.Select(r => new EntityFactoryTemplate(_settings, r).Name);

        private string InstantiableEntityInterface => _settings.InstantiableEntityInterface;
        private string ModelInterface => _settings.ModelInterface;

        private IEnumerable<EntityDefinition> NonAbstractEntities => _schema.Entities.Where(d => d.Instantiable);

        private IEnumerable<DefinedType> DefinedTypes => _schema.Types;

        private IEnumerable<string> SchemasIds => new[] { _schema.Identification };
    }
}
