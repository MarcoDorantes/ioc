using System;
using System.Linq;
using System.Collections.Generic;

namespace nutility
{
  public class TypeClassName
  {
    public TypeClassName(string name)
    {
      Name = name;
    }

    public string Name { get; private set; }

    public static implicit operator TypeClassName(string name) => new TypeClassName(name);
  }

  /// <summary>
  /// Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM IUnknown::QueryInterface method, this class follows
  /// such design tradition and relies on basic equivalent mechanisms from .NET Framework (System.IServiceProvider interface).
  /// </summary>
  public class TypeClassMapper : ITypeClassMapper
  {
    /// <summary>
    /// Keep basic Type - Class mapping.
    /// </summary>
    private IDictionary<string, TypeClassName> typemap;

    /// <summary>
    /// Type-Creator mapping.
    /// </summary>
    private IDictionary<string, Func<object>> typecreatormap;

    private Dictionary<string, object> values;

    /// <summary>
    /// Name of the configured Scope.
    /// </summary>
    private string scope;

    /// <summary>
    /// Name of the section in configuration file.
    /// </summary>
    private string section;

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
      this.typecreatormap = new Dictionary<string, Func<object>>();
      this.values = new Dictionary<string, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typeclassmap">Type-Class map</param>
    public TypeClassMapper(IDictionary<string, TypeClassName> typeclassmap)
    {
      if (typeclassmap == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(typeclassmap)}", new ArgumentNullException(nameof(typeclassmap)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typemap = new Dictionary<string, TypeClassName>(typeclassmap);
      this.typecreatormap = new Dictionary<string, Func<object>>();
      this.values = new Dictionary<string, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typeclassmap">Type-Class map</param>
    public TypeClassMapper(IDictionary<Type, TypeClassName> typeclassmap)
    {
      if (typeclassmap == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(typeclassmap)}", new ArgumentNullException(nameof(typeclassmap)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typemap = typeclassmap.Aggregate(new Dictionary<string, TypeClassName>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
      this.typecreatormap = new Dictionary<string, Func<object>>();
      this.values = new Dictionary<string, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typeclassmap">Type-Class map</param>
    public TypeClassMapper(IDictionary<Type, Type> typeclassmap)
    {
      if (typeclassmap == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(typeclassmap)}", new ArgumentNullException(nameof(typeclassmap)));
      }
      this.scope = "<explicit>";
      this.section = "<explicit>";
      this.typemap = typeclassmap.Aggregate(new Dictionary<string, TypeClassName>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value.AssemblyQualifiedName); return whole; });
      this.typecreatormap = new Dictionary<string, Func<object>>();
      this.values = new Dictionary<string, object>();
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typeclassmap">Type-Class map</param>
    /// <param name="typecreatormap">Type-Creator map</param>
    public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<Type, Func<object>> typecreatormap) : this(typeclassmap)
    {
      this.typecreatormap = typecreatormap.Aggregate(new Dictionary<string, Func<object>>(), (whole, next) => { whole.Add(next.Key.FullName, next.Value); return whole; });
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typeclassmap">Type-Class map</param>
    /// <param name="typecreatormap">Type-Creator map</param>
    public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<string, Func<object>> typecreatormap) : this(typeclassmap)
    {
      this.typecreatormap = new Dictionary<string, Func<object>>(typecreatormap);
    }

    public TypeClassMapper(IDictionary<Type, Type> typeclassmap, IDictionary<string, object> values) : this(typeclassmap)
    {
      this.values = new Dictionary<string, object>(values);
    }

    /// <summary>
    /// Existing type-class mappings.
    /// </summary>
    public IEnumerable<KeyValuePair<string, TypeClassName>> Mappings { get { return typemap; } }

    /// <summary>
    /// Given a Type, it returns its configured/mapped Class.
    /// </summary>
    /// <param name="requiredType">Required Type</param>
    /// <returns>Mapped Class</returns>
    public virtual object GetService(Type requiredType)
    {
      if (requiredType == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(requiredType)}", new ArgumentNullException(nameof(requiredType)));
      }
      object mapped_value = null;
      if (typecreatormap.ContainsKey(requiredType.FullName))
      {
        if (typecreatormap[requiredType.FullName] != null)
        {
          try
          {
            mapped_value = typecreatormap[requiredType.FullName]();
          }
          catch (Exception exception)
          {
            throw new TypeClassMapperException($"Class creator throws for Type [{requiredType}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]. Check InnerException.", exception);
          }
        }
      }
      else
      {
        if (!typemap.ContainsKey(requiredType.FullName))
        {
          throw new TypeClassMapperException($"Type not found: [{requiredType.FullName}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
        }
        mapped_value = typemap[requiredType.FullName];
        if (mapped_value is string)
        {
          mapped_value = CreateInstanceOfMappedClass(mapped_value as string, requiredType);
        }
      }
      return mapped_value;
    }

    /// <summary>
    /// For a given tradeoff between little syntactic sugar and the dependency to the ITypeClassMapper interface, this method would be the generic version of System.IServiceProvider.GetService method.
    /// </summary>
    /// <typeparam name="T">Required Type</typeparam>
    /// <returns>Mapped Class</returns>
    public T GetService<T>() => (T)this.GetService(typeof(T));

    public void AddMapping(Type type, TypeClassName name)
    {
      if (type == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(type)}", new ArgumentNullException(nameof(type)));
      }
      this.typemap.Add(type.FullName, name);
    }

    public void AddMapping(Type type, object value)
    {
      if (type == null)
      {
        throw new TypeClassMapperException($"Parameter cannot be null: {nameof(type)}", new ArgumentNullException(nameof(type)));
      }
      this.values.Add(type.FullName, value);
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
    private object CreateInstanceOfMappedClass(string classname, Type requiredType)
    {
      if (string.IsNullOrWhiteSpace(classname))
      {
        throw new TypeClassMapperException($"Mapped class for type [{requiredType.FullName}] cannot be empty: [{classname}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
      }
      Type mappedClass = GetClass(classname);
      if (mappedClass == null)
      {
        throw new TypeClassMapperException($"Mapped class for type [{requiredType.FullName}] not found: [{classname}] at configured scope [{scope ?? "<default>"}] and section [{section ?? "<default>"}]");
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
      typemap = new Dictionary<string, TypeClassName>();
      if (string.IsNullOrWhiteSpace(scope))
      {
        foreach (MappingCollectionElement mapping in configSection.Mappings)
        {
          typemap.Add(mapping.Type, mapping.Class);
        }
      }
      else
      {
        foreach (MappingCollectionElement mapping in configSection.Scopes.GetScopeMappings(scope))
        {
          typemap.Add(mapping.Type, mapping.Class);
        }
      }
    }
  }
}