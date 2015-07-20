using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.ExpressParser.SDAI
{
    public class SchemaModel
    {
        private readonly EntityDictionary _entities = new EntityDictionary();

        public PredefinedSimpleTypes PredefinedSimpleTypes { get; private set; }

        public SchemaModel(bool init = true)
        {
            if (!init) return;

            Schema = new SchemaDefinition { SchemaModel = this };
            _entities.Add(Schema);

            PredefinedSimpleTypes = new PredefinedSimpleTypes(this);
        }

        /// <summary>
        /// You should always use this function to create new entities in the schema
        /// </summary>
        /// <typeparam name="T">Type of the object to be created</typeparam>
        /// <param name="initialization">Optional initialization routine for the object</param>
        /// <returns>New object of the defined type</returns>
        public T New<T>(Action<T> initialization = null) where T : SchemaEntity, new()
        {
            if (typeof(T) == typeof(SchemaDefinition))
                throw new Exception("SchemaDefinition is expected to be defined only once. That is done automatically in constructor of the model.");

            var result = new T {SchemaModel = this};
            if (initialization != null)
                initialization(result);

            //set parent schema if it is a named type and schema is defined
            var namedType = result as NamedType;
            if (namedType != null)
                namedType.ParentSchema = Schema;
            var rule = result as GlobalRule;
            if (rule != null)
                rule.ParentSchema = Schema;

            _entities.Add(result);
            return result;
        }

        /// <summary>
        /// Gets all schema entities from the model
        /// </summary>
        /// <typeparam name="T">Type of entities</typeparam>
        /// <param name="predicate">Optional predicate to select only some</param>
        /// <returns>Enumeration of entities of defined type</returns>
        public IEnumerable<T> Get<T>(Func<T, bool> predicate = null) where T : class, ISchemaEntity
        {
            return predicate == null ? _entities.Where<T>() : _entities.Where(predicate);
        }

        /// <summary>
        /// First definition in the model. There should only be one in the model.
        /// </summary>
        public SchemaDefinition Schema
        {
            get; private set; 
        }

        public static SchemaModel LoadIfc2x3()
        {
            return Load(Schemas.Schemas.IFC2X3_TC1);
        }

        public static SchemaModel LoadIfc4()
        {
            return  Load(Schemas.Schemas.IFC4);
        }

        public static SchemaModel Load(string schema)
        {
            var parser = new ExpressParser();
            var result = parser.Parse(schema);
            if (!result)
                throw new Exception("Unexpected parser error: " + parser.Errors.LastOrDefault());
            return parser.SchemaInstance;
        }
    }

    internal class EntityDictionary : IDictionary<Type, List<ISchemaEntity>>
    {
        private readonly Dictionary<Type, List<ISchemaEntity>> _internal = new Dictionary<Type, List<ISchemaEntity>>();

        private static bool IsValidType(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && typeof (ISchemaEntity).IsAssignableFrom(type);
        }

        public IEnumerable<T> Where<T>(Func<T, bool> predicate = null) where T : class, ISchemaEntity
        {
            var queryType = typeof (T);
            var resultTypes = _internal.Keys.Where(t => queryType.IsAssignableFrom(t));
            return
                resultTypes.SelectMany(type => _internal[type], (type, entity) => entity as T)
                    .Where(result => predicate == null || predicate(result));
        }

        public void Add(ISchemaEntity entity)
        {
            Add(entity.GetType(), new List<ISchemaEntity> {entity});
        }

        /// <summary>
        /// Only concrete types can be added
        /// </summary>
        /// <param name="key">Key which must be concrete type of schema entity</param>
        /// <param name="value"></param>
        public void Add(Type key, List<ISchemaEntity> value)
        {
            if (!IsValidType(key))
                throw new NotSupportedException("Key must be a concrete type of SchemaEntity");

            if (_internal.ContainsKey(key))
                _internal[key].AddRange(value);
            else
                _internal.Add(key, value);
        }

        public bool ContainsKey(Type key)
        {
            return _internal.ContainsKey(key);
        }

        public ICollection<Type> Keys
        {
            get { return _internal.Keys; }
        }

        public bool Remove(Type key)
        {
            return _internal.Remove(key);
        }

        public bool TryGetValue(Type key, out List<ISchemaEntity> value)
        {
            return _internal.TryGetValue(key, out value);
        }

        public ICollection<List<ISchemaEntity>> Values
        {
            get { return _internal.Values; }
        }

        public List<ISchemaEntity> this[Type key]
        {
            get
            {
                if (!IsValidType(key))
                    throw new NotSupportedException("Key must be a concrete type of SchemaEntity");

                if (_internal.ContainsKey(key))
                    return _internal[key];
                else
                {
                    var result = new List<ISchemaEntity>();
                    _internal.Add(key, result);
                    return result;
                }
            }
            set
            {
                if (!IsValidType(key))
                    throw new NotSupportedException("Key must be a concrete type of SchemaEntity");
                if (_internal.ContainsKey(key))
                    _internal[key] = value;
                else
                    _internal.Add(key, value);
            }
        }

        public void Add(KeyValuePair<Type, List<ISchemaEntity>> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _internal.Clear();
        }

        public bool Contains(KeyValuePair<Type, List<ISchemaEntity>> item)
        {
            return _internal.Contains(item);
        }

        public void CopyTo(KeyValuePair<Type, List<ISchemaEntity>>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Total count of SchemaEntities of all types
        /// </summary>
        public int Count
        {
            get { return _internal.Values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<Type, List<ISchemaEntity>> item)
        {
            if (_internal.ContainsKey(item.Key) && _internal[item.Key] == item.Value)
                return _internal.Remove(item.Key);

            throw new Exception("This item is not an item of this doctionary;");
        }

        public IEnumerator<KeyValuePair<Type, List<ISchemaEntity>>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internal.GetEnumerator();
        }
    }

    public class PredefinedSimpleTypes
    {
        internal PredefinedSimpleTypes(SchemaModel model)
        {
            IntegerType = model.New<IntegerType>();
            RealType = model.New<RealType>();
            StringType = model.New<StringType>();
            BinaryType = model.New<BinaryType>();
            LogicalType = model.New<LogicalType>();
            BooleanType = model.New<BooleanType>();
            NumberType = model.New<NumberType>();
        }

        public IntegerType IntegerType { get; private set; }
        public RealType RealType { get; private set; }
        public StringType StringType { get; private set; }
        public BinaryType BinaryType { get; private set; }
        public LogicalType LogicalType { get; private set; }
        public BooleanType BooleanType { get; private set; }
        public NumberType NumberType { get; private set; }
    }
}