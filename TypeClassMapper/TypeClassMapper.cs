using System;
using System.Collections.Generic;

namespace utility
{
  /// <summary>
  /// Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM IUnknown::QueryInterface method, this class follows
  /// such design tradition and relies on basic equivalent mechanisms from .NET Framework.
  /// </summary>
  public class TypeClassMapper : IServiceProvider
  {
    /// <summary>
    /// Keep basic Type - Class mapping.
    /// </summary>
    private IDictionary<string, string> typemap;

    /// <summary>
    /// For implicit, config-based, type-class mappings.
    /// </summary>
    public TypeClassMapper(string scope = null, string section = null)
    {
      TypeClassMapperConfigurationSection configSection = GetConfiguration(section);
      InitAndLoadMappings(configSection, scope);
    }

    /// <summary>
    /// For explicit type-class mappings.
    /// </summary>
    /// <param name="typemap">Type-Class map</param>
    public TypeClassMapper(IDictionary<string, string> typemap)
    {
      if (typemap == null)
      {
        throw new ArgumentNullException(nameof(typemap));
      }
      this.typemap = typemap;
    }

    /// <summary>
    /// Given a Type, it returns its configured/mapped Class.
    /// </summary>
    /// <param name="serviceType">Required Type</param>
    /// <returns>Mapped Class</returns>
    public virtual object GetService(Type serviceType)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException(nameof(serviceType));
      }
      if (!typemap.ContainsKey(serviceType.FullName))
      {
        throw new Exception($"Type not found: {serviceType.FullName}");
      }
      return Activator.CreateInstance(BasicGetType(typemap[serviceType.FullName]));
    }

    /// <summary>
    /// Uses the most basic .NET mechanism to Type/Class resolution (including in-process and also assembly resolution).
    /// </summary>
    /// <param name="typename">Fully qualified Type name</param>
    /// <returns>Required Type</returns>
    internal Type BasicGetType(string typename)
    {
      return Type.GetType(typename);
    }

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
      typemap = new Dictionary<string, string>();
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

  #region ConfigSection
  public class TypeClassMapperConfigurationSection : System.Configuration.ConfigurationSection
  {
    [System.Configuration.ConfigurationProperty("Mappings", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(MappingCollection), AddItemName = "Mapping")]
    public MappingCollection Mappings { get { return (MappingCollection)this["Mappings"]; } }

    [System.Configuration.ConfigurationProperty("Scopes", IsRequired = false)]
    [System.Configuration.ConfigurationCollection(typeof(ScopeCollection), AddItemName = "Scope")]
    public ScopeCollection Scopes { get { return (ScopeCollection)this["Scopes"]; } }
  }
  public class MappingCollection : System.Configuration.ConfigurationElementCollection
  {
    protected override System.Configuration.ConfigurationElement CreateNewElement()
    {
      return new MappingCollectionElement();
    }
    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
    {
      return ((MappingCollectionElement)element).Type;
    }
  }
  public class MappingCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Type", IsRequired = true)]
    public string Type { get { return (string)this["Type"]; } set { this["Type"] = value; } }

    [System.Configuration.ConfigurationProperty("Class", IsRequired = true)]
    public string Class { get { return (string)this["Class"]; } set { this["Class"] = value; } }
  }
  public class ScopeCollection : System.Configuration.ConfigurationElementCollection
  {
    protected override System.Configuration.ConfigurationElement CreateNewElement()
    {
      return new ScopeCollectionElement();
    }
    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
    {
      return ((ScopeCollectionElement)element).Name;
    }
    internal MappingCollection GetScopeMappings(string scope_name)
    {
      foreach (ScopeCollectionElement scope in this)
      {
        if (scope.Name != scope_name) continue;
        return scope.Mappings;
      }
      throw new System.Configuration.ConfigurationErrorsException($"Scope name [{scope_name}] not found.");
    }
  }
  public class ScopeCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Name", IsRequired = true)]
    public string Name { get { return (string)this["Name"]; } set { this["Name"] = value; } }

    [System.Configuration.ConfigurationProperty("Mappings", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(MappingCollection), AddItemName = "Mapping")]
    public MappingCollection Mappings { get { return (MappingCollection)this["Mappings"]; } }
  }

  #endregion
}