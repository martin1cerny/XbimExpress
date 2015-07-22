using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Templates.Infrastructure
{
    public partial class EntityFactoryTemplate: ICodeTemplate
    {
        private readonly GeneratorSettings _settings;
        private readonly SchemaModel _schema;

        public EntityFactoryTemplate(GeneratorSettings settings, SchemaModel schema)
        {
            _settings = settings;
            _schema = schema;
        }

        public string Name
        {
            get { return "EntityFactory"; }
        }

        public string Namespace
        {
            get { return _settings.Namespace; }
        }

        public string Inheritance
        {
            get { return null; }
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

                return result;
                
            }
        }

        private string PersistEntity { get { return _settings.PersistEntityInterface; } }
        private string ModelInterface { get { return _settings.ModelInterface; } }

        private IEnumerable<EntityDefinition> NonAbstractEntities { get
        {
            return _schema.Get<EntityDefinition>(d => d.Instantiable);
        } } 
    }
}
