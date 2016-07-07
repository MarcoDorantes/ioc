namespace utility
{
  public class TypeClassMapperConfigurationSection : System.Configuration.ConfigurationSection
  {
    [System.Configuration.ConfigurationProperty("Mappings", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(MappingCollection), AddItemName = "Mapping")]
    public MappingCollection Mappings { get { return (MappingCollection)this["Mappings"]; } }

    [System.Configuration.ConfigurationProperty("Scopes", IsRequired = false)]
    [System.Configuration.ConfigurationCollection(typeof(ScopeCollection), AddItemName = "Scope")]
    public ScopeCollection Scopes { get { return (ScopeCollection)this["Scopes"]; } }
  }
}