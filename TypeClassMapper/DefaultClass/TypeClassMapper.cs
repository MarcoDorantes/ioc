using System;
using System.Linq;
using System.Collections.Generic;

namespace nutility
{
  public class Mapping
  {
    //public TypeClassID RequiredProgID;? Versioning? Trying to bring a bad past pattern to a already superior present solution?
    public TypeClassID RequiredType;
    public TypeClassID ClientType;
    public TypeClassID MappedClass;
  }

  public class MappedTypes //MappedTuple? MappingTuple?
  {
    public Type RequiredType;
    public Type ClientType;
    public Type MappedClass;
  }

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

    private IDictionary<TypeClassID, object> typeobjectmap;

    /// <summary>
    /// Type-Creator mapping.
    /// </summary>
    private IDictionary<TypeClassID, Func<object>> typecreatormap;

    private Dictionary<TypeClassID, object> values;

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
      InitAndLoadMappings(configSection, scope);
      this.typeobjectmap = new Dictionary<TypeClassID, object>();
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IDictionary<TypeClassID, TypeClassID> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key, ClientType = null, MappedClass = next.Value }); return whole; });
      this.typeobjectmap = new Dictionary<TypeClassID, object>();
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IDictionary<Type, TypeClassID> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value }); return whole; });
      this.typeobjectmap = new Dictionary<TypeClassID, object>();
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map)
    {
      if (Type_Class_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Class_Map)}", new ArgumentNullException(nameof(Type_Class_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typeclass_catalog = Type_Class_Map.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping() { RequiredType = next.Key.FullName, ClientType = null, MappedClass = next.Value.AssemblyQualifiedName }); return whole; });
      this.typeobjectmap = new Dictionary<TypeClassID, object>();
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Object_Map">Type-Object map</param>
    public TypeClassMapper(IDictionary<TypeClassID, object> Type_Object_Map)
    {
      if (Type_Object_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typeclass_catalog = new List<Mapping>();
      this.typeobjectmap = new Dictionary<TypeClassID, object>(Type_Object_Map);
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Object_Map">Type-Object map</param>
    public TypeClassMapper(IDictionary<Type, object> Type_Object_Map)
    {
      if (Type_Object_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typeclass_catalog = new List<Mapping>();
      this.typeobjectmap = Type_Object_Map.Aggregate(new Dictionary<TypeClassID, object>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>();
      this.values = new Dictionary<TypeClassID, object>();
    }

    public TypeClassMapper(IDictionary<Type, TypeClassID> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      //TODO cannot do: this(typeclassmap) && this(typeobjectmap) -so, design a common init logic accordingly.
      if (Type_Object_Map == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(Type_Object_Map)}", new ArgumentNullException(nameof(Type_Object_Map)));
      }
      this.typeobjectmap = Type_Object_Map.Aggregate(new Dictionary<TypeClassID, object>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
    }

    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map, IEnumerable<KeyValuePair<Type, object>> Type_Object_Map) : this(Type_Class_Map)
    {
      this.typeobjectmap = Type_Object_Map.Aggregate(new Dictionary<TypeClassID, object>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    /// <param name="Type_Creator_Map">Type-Creator map</param>
    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map, IDictionary<Type, Func<object>> Type_Creator_Map) : this(Type_Class_Map)
    {
      this.typecreatormap = Type_Creator_Map.Aggregate(new Dictionary<TypeClassID, Func<object>>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="Type_Class_Map">Type-Class map</param>
    /// <param name="Type_Creator_Map">Type-Creator map</param>
    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map, IDictionary<TypeClassID, Func<object>> Type_Creator_Map) : this(Type_Class_Map)
    {
      this.typecreatormap = new Dictionary<TypeClassID, Func<object>>(Type_Creator_Map);
    }

    public TypeClassMapper(IDictionary<TypeClassID, TypeClassID> Type_Class_Map, IDictionary<TypeClassID, object> Type_Value_Map) : this(Type_Class_Map)
    {
      InitializeValues(Type_Value_Map);
    }

    public TypeClassMapper(IDictionary<Type, TypeClassID> Type_Class_Map, IDictionary<TypeClassID, object> Type_Value_Map) : this(Type_Class_Map)
    {
      InitializeValues(Type_Value_Map);
    }

    public TypeClassMapper(IDictionary<Type, Type> Type_Class_Map, IDictionary<TypeClassID, object> Type_Value_Map) : this(Type_Class_Map)
    {
      InitializeValues(Type_Value_Map);
    }

    public TypeClassMapper(IEnumerable<MappedTypes> Type_Class_Catalog)// : this(typeclassmap)
    {
      //TODO cannot do: this(typeclassmap) && this(typeobjectmap) -so, design a common init logic accordingly.

      this.typeclass_catalog = Type_Class_Catalog.Aggregate(new List<Mapping>(), (whole, next) => { whole.Add(new Mapping { RequiredType = next.RequiredType.FullName, ClientType = next.ClientType?.FullName, MappedClass = next.MappedClass.AssemblyQualifiedName }); return whole; });
//      this.values = new Dictionary<TypeClassID, object>(values);
    }

    public TypeClassMapper(IEnumerable<Mapping> Type_Class_Catalog)// : this(typeclassmap)
    {
      this.typeclass_catalog = new List<Mapping>(Type_Class_Catalog);
//      this.values = new Dictionary<TypeClassID, object>(values);
    }

    /// <summary>
    /// Existing type-class mappings.
    /// </summary>
    public IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Mappings { get { return typeclass_catalog.Aggregate(new Dictionary<TypeClassID, TypeClassID>(), (whole, next) => { whole.Add(next.RequiredType, next.MappedClass); return whole; }); } }

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
      this.values.Add(name, value);
    }

    public T GetValue<T>(string name)
    {
      T result = default(T);
      if (this.values.ContainsKey(name))
      {
        result = (T)this.values[name];
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

    private void InitAndLoadMappings(TypeClassMapperConfigurationSection configSection, string scope = null)
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

    private void InitializeValues(IDictionary<TypeClassID, object> values)
    {
      if (values == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(values)}", new ArgumentNullException(nameof(values)));
      }
      this.values = new Dictionary<TypeClassID, object>(values);
    }
  }
}