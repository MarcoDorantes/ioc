using System;
using System.Linq;
using System.Collections.Generic;

namespace nutility
{
  /// <summary>
  /// Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM IUnknown::QueryInterface method, this class follows
  /// such design tradition and relies on basic equivalent mechanisms from .NET Framework (System.IServiceProvider interface).
  /// </summary>
  public class TypeClassMapper : ITypeClassMapper
  {
    /// <summary>
    /// Keep Type - Class catalog.
    /// </summary>
    private IList<Mapping> typeclass_catalog;


    /// <summary>
    /// Keep Type - Object map.
    /// </summary>
    private IDictionary<TypeClassID, object> typeobjectmap;

    /// <summary>
    /// Type-Creator mapping.
    /// </summary>
    private IDictionary<TypeClassID, Func<object>> typecreatormap;

    /// <summary>
    /// Keep Name - Object map.
    /// </summary>
    private IDictionary<string, object> nameobjectmap;

    /// <summary>
    /// Name of the configured Scope.
    /// </summary>
    private string scope;

    /// <summary>
    /// Name of the section in configuration file.
    /// </summary>
    private string section;

    #region ctors
    /*
        public TypeClassMapper(string scope = null, string section = null)


        public TypeClassMapper(IDictionary<string, TypeClassName> typeclassmap)

        public TypeClassMapper(IDictionary<Type, TypeClassName> typeclassmap)

        public TypeClassMapper(IDictionary<Type, Type> typeclassmap)


        public TypeClassMapper(IDictionary<string, object> typeobjectmap)

        public TypeClassMapper(IDictionary<Type, object> typeobjectmap)


        public TypeClassMapper(IDictionary<Type, TypeClassName> typeclassmap, IEnumerable<KeyValuePair<Type, object>> typeobjectmap)

    -
        public TypeClassMapper(IDictionary<string, TypeClassName> typeclassmap, IEnumerable<KeyValuePair<string, object>> typeobjectmap)

        public TypeClassMapper(IDictionary<string, TypeClassName> typeclassmap, IEnumerable<KeyValuePair<Type, object>> typeobjectmap)


        public TypeClassMapper(IDictionary<Type, TypeClassName> typeclassmap, IEnumerable<KeyValuePair<string, object>> typeobjectmap)


        public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<string, object> typeobjectmap)

        public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<Type, object> typeobjectmap)

    -

        public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<Type, Func<object>> typecreatormap) : this(typeclassmap)

        public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<string, Func<object>> typecreatormap) : this(typeclassmap)


        public TypeClassMapper(IDictionary<string, TypeClassName> typeclassmap, IDictionary<string, object> values) : this(typeclassmap)

        public TypeClassMapper(IDictionary<Type, TypeClassName> typeclassmap, IDictionary<string, object> values) : this(typeclassmap)

        public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<string, object> values) : this(typeclassmap)
    */
    #endregion

    /// <summary>
    /// For implicit, config-based, type-class mappings.
    /// </summary>
    /// <param name="scope">Name of the configured Scope. Default means required <see cref="MappingCollection"/> configured element.</param>
    /// <param name="section">Name of the section in configuration file. Default means the <see cref="TypeClassMapperConfigurationSection"/> class name.</param>
    public TypeClassMapper(string scope = null, string section = null)
    {
      this.scope = scope;
      this.section = section;
      TypeClassMapperConfigurationSection configSection = GetConfiguration(section);
      InitAndLoadMappingsFrom(configSection, scope);

      InitializeTypeCreatorMap<TypeClassID>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Type_Class_Map)
    {
      this.scope = "<explicit>";
      this.section = "<explicit>";

      InitializeTypeClassCatalog<TypeClassID, TypeClassID>(Type_Class_Map, (whole, next) => { whole.Add(new Mapping { RequiredType = next.Key, ClientType = null, MappedClass = next.Value }); });
      //this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key, ClientType = null, MappedClass = next.Value }); return whole; });
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, Type>> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      if (Type_Class_Map.Any(mapping => mapping.Key == null))
      {
        throw new TypeClassMapperException($"Required {nameof(TypeClassID)} in mapping cannot be null.");
      }
      if (Type_Class_Map.Any(mapping => mapping.Value == null))
      {
        throw new TypeClassMapperException($"Mapped {nameof(TypeClassID)} in mapping cannot be null.");
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";

      InitializeTypeClassCatalog<TypeClassID, Type>(Type_Class_Map, (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key, ClientType = null, MappedClass = next.Value.AssemblyQualifiedName }); });
        //this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key, ClientType = null, MappedClass = next.Value.AssemblyQualifiedName }); return whole; });
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, Type>> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";


      InitializeTypeClassCatalog<Type, Type>(Type_Class_Map, (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value.AssemblyQualifiedName }); });
      //this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value.AssemblyQualifiedName }); return whole; });
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, TypeClassID>> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";


      InitializeTypeClassCatalog<Type, TypeClassID>(Type_Class_Map, (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value }); });
      //this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value }); return whole; });
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Object_Map">Type-Object map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map)
    {
      if (Type_Object_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";

      InitializeTypeClassCatalog<Type, TypeClassID>(null, null, new List<Mapping>());
      //this.typeclass_catalog = new List<Mapping>();
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Object_Map">Type-Object map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, object>> Type_Object_Map)
    {
      if (Type_Object_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";

      InitializeTypeClassCatalog<Type, TypeClassID>(null, null, new List<Mapping>());
      //this.typeclass_catalog = new List<Mapping>();
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<Type>(Type_Object_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Type_Class_Map, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<Type>(Type_Object_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, Type>> Type_Class_Map, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<TypeClassID, Type>> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<Type>(Type_Object_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, TypeClassID>> Type_Class_Map, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, TypeClassID>> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      //TODO cannot do: this(typeclassmap) && this(typeobjectmap) -so, design a common init logic accordingly.
      /*if (Type_Object_Map == null)
            {
              throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
            }
            this.typeobjectmap = Type_Object_Map.Aggregate(new Dictionary<TypeClassID, object>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });*/
      InitializeTypeObjectMap<Type>(Type_Object_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, Type>> Type_Class_Map, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, Type>> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      InitializeTypeObjectMap<Type>(Type_Object_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    /// <param name="Type_Creator_Map">Type-Creator map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, Type>> Type_Class_Map, IEnumerable<KeyValuePair<Type, Func<object>>> Type_Creator_Map) : this(Type_Class_Map)
    {
      InitializeTypeCreatorMap<Type>(Type_Creator_Map, (whole, next) => { whole.Add(next.Key.FullName, next.Value); });
    }

    /// <summary>
    /// Type-object_creator mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    /// <param name="Type_Creator_Map">Type-Creator map</param>
    public TypeClassMapper(IEnumerable<KeyValuePair<Type, Type>> Type_Class_Map, IEnumerable<KeyValuePair<TypeClassID, Func<object>>> Type_Creator_Map) : this(Type_Class_Map)
    {
      InitializeTypeCreatorMap<TypeClassID>(Type_Creator_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }

    public TypeClassMapper(IDictionary<TypeClassID, TypeClassID> Type_Class_Map, IEnumerable<KeyValuePair<string, object>> Name_Object_Map) : this(Type_Class_Map)
    {
      InitializeNameObjectMap(Name_Object_Map);
    }

    public TypeClassMapper(IDictionary<Type, TypeClassID> Type_Class_Map, IEnumerable<KeyValuePair<string, object>> Name_Object_Map) : this(Type_Class_Map)
    {
      InitializeNameObjectMap(Name_Object_Map);
    }

    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map, IEnumerable<KeyValuePair<string, object>> Name_Object_Map) : this(Type_Class_Map)
    {
      InitializeNameObjectMap(Name_Object_Map);
    }


    public TypeClassMapper(IEnumerable<Mapping> Type_Class_Catalog)// : this(typeclassmap)
    {
      //new nutility.Mapping { RequiredType = null

      InitializeTypeClassCatalog<Type, TypeClassID>(null, null, new List<Mapping>(Type_Class_Catalog));
      //this.typeclass_catalog = new List<Mapping>(Type_Class_Catalog);
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    public TypeClassMapper(IEnumerable<MappedTypes> Type_Class_Catalog)// : this(typeclassmap)
    {
      //TODO cannot do: this(typeclassmap) && this(typeobjectmap) -so, design a common init logic accordingly.

      InitializeTypeClassCatalog<Type, TypeClassID>(null, null, Type_Class_Catalog.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping { RequiredType = next.RequiredType.FullName, ClientType = next.ClientType?.FullName, MappedClass = next.MappedClass.AssemblyQualifiedName }); return whole; }));
      //this.typeclass_catalog = Type_Class_Catalog.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping { RequiredType = next.RequiredType.FullName, ClientType = next.ClientType?.FullName, MappedClass = next.MappedClass.AssemblyQualifiedName }); return whole; });
      InitializeTypeCreatorMap<Type>(null, null, new Dictionary<TypeClassID, Func<object>>());
      InitializeTypeObjectMap<TypeClassID>(null, null, new Dictionary<TypeClassID, object>());
      InitializeNameObjectMap(null, new Dictionary<string, object>());
    }

    public TypeClassMapper(IEnumerable<MappedTypes> Type_Class_Catalog, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Catalog)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }

    public TypeClassMapper(IEnumerable<Mapping> Type_Class_Catalog, IEnumerable<KeyValuePair<TypeClassID, object>> Type_Object_Map) : this(Type_Class_Catalog)
    {
      InitializeTypeObjectMap<TypeClassID>(Type_Object_Map, (whole, next) => { whole.Add(next.Key, next.Value); });
    }

    public TypeClassMapper(IEnumerable<Mapping> Type_Class_Catalog, IEnumerable<KeyValuePair<string,object>> Name_Object_Map) : this(Type_Class_Catalog)
    {
      InitializeNameObjectMap(Name_Object_Map);
    }

    /// <summary>
    /// A copy of the existing type-class mappings.
    /// </summary>
    public IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Mappings => typeclass_catalog.Aggregate(new Dictionary<TypeClassID, TypeClassID>(), (whole, next) => { whole.Add(next.RequiredType, next.MappedClass); return whole; });

    /// <summary>
    /// A copy of the existing type-class catalog.
    /// </summary>
    public IEnumerable<Mapping> Catalog => typeclass_catalog.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping(next)); return whole; });

    /// <summary>
    /// A copy of the existing name-object values.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object>> Values => new Dictionary<string, object>(nameobjectmap);

    /// <summary>
    /// Given a Type, it returns its configured/mapped Class.
    /// </summary>
    /// <param name="Required_Type">Required Type</param>
    /// <returns>Mapped Class</returns>
    public virtual object GetService(Type Required_Type)
    {
      if (Required_Type == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Required_Type)}", new ArgumentNullException(nameof(Required_Type)));
      }
      object mapped_instance = null;
      if (typecreatormap.ContainsKey(Required_Type.FullName))
      {
        if (typecreatormap[Required_Type.FullName] != null)
        {
          try
          {
            mapped_instance = typecreatormap[Required_Type.FullName]();
          }
          catch (Exception exception)
          {
            throw new TypeClassMapperException($"Class creator throws for Type [{Required_Type}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]. Check InnerException.", exception);
          }
        }
      }
      else if (typeobjectmap.ContainsKey(Required_Type.FullName))
      {
        mapped_instance = typeobjectmap[Required_Type.FullName];
      }
      else
      {
        if (!typeclass_catalog.Any(m => m.RequiredType.ID == Required_Type.FullName))
        {
          throw new TypeClassMapperException($"Type not found: [{Required_Type.FullName}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
        }
        TypeClassID mapped_classname = typeclass_catalog.First(m => m.RequiredType.ID == Required_Type.FullName).MappedClass;
        mapped_instance = CreateInstanceOfMappedClass(mapped_classname, Required_Type);
      }
      return mapped_instance;
    }

    /// <summary>
    /// For a given tradeoff between little syntactic sugar and the dependency to the ITypeClassMapper interface, this method would be the generic version of System.IServiceProvider.GetService method.
    /// </summary>
    /// <typeparam name="T">Required Type</typeparam>
    /// <returns>Mapped Class</returns>
    public T GetService<T>() => (T)this.GetService(typeof(T));

    public T GetService<T>(Type Client_Type)
    {
      T mapped_instance = default(T);
      var mapping = typeclass_catalog.FirstOrDefault(m => m.RequiredType.ID == typeof(T).FullName && m.ClientType?.ID == Client_Type?.FullName);
      if (mapping != null)
      {
        mapped_instance = (T)CreateInstanceOfMappedClass(mapping.MappedClass, typeof(T));
      }
      return mapped_instance;
    }

    public T GetService<T>(TypeClassID Client_Type)
    {
      T mapped_instance = default(T);
      var mapping = typeclass_catalog.FirstOrDefault(m => m.RequiredType.ID == typeof(T).FullName && m.ClientType?.ID == Client_Type?.ID);
      if (mapping != null)
      {
        mapped_instance = (T)CreateInstanceOfMappedClass(mapping.MappedClass, typeof(T));
      }
      return mapped_instance;
    }

    public void AddMapping<T>(T value)
    {
      if (typeof(T) == typeof(TypeClassID))
      {
        this.typeclass_catalog.Add(new Mapping() { RequiredType = typeof(TypeClassID).FullName, ClientType = null, MappedClass = (TypeClassID)Convert.ChangeType(value, typeof(TypeClassID)) });
      }
      else
      {
        this.typeobjectmap.Add(typeof(T).FullName, value);
      }
    }

    public void SetValue<T>(string name, T value)
    {
      this.nameobjectmap.Add(name, value);
    }

    public T GetValue<T>(string name)
    {
      T result = default(T);
      if (this.nameobjectmap.ContainsKey(name))
      {
        result = (T)this.nameobjectmap[name];
      }
      return result;
    }

    /// <summary>
    /// A new instance is created from the mapped class.
    /// </summary>
    /// <param name="classname">Name of the mapped class</param>
    /// <param name="requiredType">Required type</param>
    /// <returns></returns>
    private object CreateInstanceOfMappedClass(TypeClassID classname, Type requiredType)
    {
      if (classname == null)
      {
        throw new TypeClassMapperException($"Mapped class for type [{requiredType.FullName}] cannot be null: at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
      }
      if (string.IsNullOrWhiteSpace(classname.ID))
      {
        throw new TypeClassMapperException($"Mapped class for type [{requiredType.FullName}] cannot be empty: [{classname.ID}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
      }
      Type mappedClass = GetClass(classname.ID);
      if (mappedClass == null)
      {
        throw new TypeClassMapperException($"Mapped class for type [{requiredType.FullName}] not found: [{classname.ID}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
      }
      try
      {
        if (HasTypeClassMapConstructor(mappedClass))
        {
          return System.Activator.CreateInstance(mappedClass, this);
        }
        else
        {
          return System.Activator.CreateInstance(mappedClass);
        }
      }
      catch (System.Reflection.TargetInvocationException exception)
      {
        throw new TypeClassMapperException($"Cannot create an instance of [{mappedClass}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]. Check InnerException.", exception.InnerException);
      }
      catch (Exception exception)
      {
        throw new TypeClassMapperException($"Cannot create an instance of [{mappedClass}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]. Check InnerException.", exception);
      }
    }

    /// <summary>
    /// Uses the most basic .NET mechanism to Type/Class resolution (including in-process and also assembly resolution).
    /// </summary>
    /// <param name="classname">Fully qualified Type name</param>
    /// <returns>Required Type</returns>
    private Type GetClass(string classname)
    {
      return System.Type.GetType(classname);
    }

    /// <summary>
    /// True if the mapped class has a constructor with a single parameter of type System.IServiceProvider, ITypeClassMapper or TypeClassMapper.
    /// </summary>
    /// <param name="type">Class of object to be constructed.</param>
    /// <returns>True if the mapped class has a constructor with a single parameter of type System.IServiceProvider, ITypeClassMapper or TypeClassMapper.</returns>
    private bool HasTypeClassMapConstructor(Type type) => type.GetConstructor(new Type[] { typeof(System.IServiceProvider) }) != null || type.GetConstructor(new Type[] { typeof(nutility.ITypeClassMapper) }) != null || type.GetConstructor(new Type[] { typeof(nutility.TypeClassMapper) }) != null;

    private TypeClassMapperConfigurationSection GetConfiguration(string section = null)
    {
      const string TypeClassMapperConfigurationSectionName = nameof(TypeClassMapperConfigurationSection);
      string section_name = TypeClassMapperConfigurationSectionName;
      if (!string.IsNullOrWhiteSpace(section))
      {
        section_name = section;
      }
      System.Configuration.ConfigurationManager.RefreshSection(section_name);
      TypeClassMapperConfigurationSection result = System.Configuration.ConfigurationManager.GetSection(section_name) as TypeClassMapperConfigurationSection;
      if (result == null)
      {
        throw new System.Configuration.ConfigurationErrorsException($"{TypeClassMapperConfigurationSectionName} is not properly configured.");
      }
      return result;
    }

    private void InitAndLoadMappingsFrom(TypeClassMapperConfigurationSection configSection, string scope = null)
    {
      typeclass_catalog = new List<Mapping>();
      if (string.IsNullOrWhiteSpace(scope))
      {
        foreach (MappingCollectionElement mapping in configSection.Mappings)
        {
          typeclass_catalog.Add(new Mapping() { RequiredType = mapping.Type, ClientType = null, MappedClass = mapping.Class });
        }
      }
      else
      {
        foreach (MappingCollectionElement mapping in configSection.Scopes.GetScopeMappings(scope))
        {
          typeclass_catalog.Add(new Mapping() { RequiredType = mapping.Type, ClientType = null, MappedClass = mapping.Class });
        }
      }
    }

    private void InitializeTypeClassCatalog<T, V>(IEnumerable<KeyValuePair<T, V>> Type_Class_Map, Action<IList<Mapping>, KeyValuePair<T, V>> adder, IList<Mapping> @new = null)
    {
      if (@new != null)
      {
        this.typeclass_catalog = @new;
      }
      else
      {
        if (Type_Class_Map == null)
        {
          throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
        }
        if (Type_Class_Map.Any(mapping => mapping.Key == null))
        {
          throw new TypeClassMapperException($"Required {nameof(TypeClassID)} in mapping cannot be null.");
        }
        if (Type_Class_Map.Any(mapping => mapping.Value == null))
        {
          throw new TypeClassMapperException($"Mapped {nameof(TypeClassID)} in mapping cannot be null.");
        }
        this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { adder(whole, next); return whole; });
      }
    }

    private void InitializeTypeCreatorMap<T>(IEnumerable<KeyValuePair<T, Func<object>>> Type_Creator_Map, Action<IDictionary<TypeClassID, Func<object>>, KeyValuePair<T, Func<object>>> adder, IDictionary<TypeClassID, Func<object>> @new = null)
    {
      if (@new != null)
      {
        this.typecreatormap = @new;
      }
      else
      {
        if (Type_Creator_Map == null)
        {
          throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Creator_Map)}", new ArgumentNullException(nameof(Type_Creator_Map)));
        }
        this.typecreatormap = Type_Creator_Map.Aggregate(new Dictionary<TypeClassID, Func<object>>(), (whole, next) => { adder(whole, next); return whole; });
      }
    }

    private void InitializeTypeObjectMap<T>(IEnumerable<KeyValuePair<T, object>> Type_Object_Map, Action<IDictionary<TypeClassID, object>, KeyValuePair<T, object>> adder, IDictionary<TypeClassID, object> @new = null)
    {
      if (@new != null)
      {
        this.typeobjectmap = @new;
      }
      else
      {
        if (Type_Object_Map == null)
        {
          throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
        }
        this.typeobjectmap = Type_Object_Map.Aggregate(new Dictionary<TypeClassID, object>(), (whole, next) => { adder(whole, next); return whole; });
      }
    }

    private void InitializeNameObjectMap(IEnumerable<KeyValuePair<string, object>> Name_Object_Map, IDictionary<string, object> @new = null)
    {
      if (@new != null)
      {
        this.nameobjectmap = @new;
      }
      else
      {
        if (Name_Object_Map == null)
        {
          throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Name_Object_Map)}", new ArgumentNullException(nameof(Name_Object_Map)));
        }
        this.nameobjectmap = Name_Object_Map.Aggregate(new Dictionary<string, object>(), (whole, next) => { whole.Add(next.Key, next.Value); return whole; });
      }
    }
  }
}