namespace nutility
{
  public class ScopeCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Name", IsRequired = true)]
    public string Name { get { return (string)this["Name"]; } set { this["Name"] = value; } }

    [System.Configuration.ConfigurationProperty("Mappings", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(MappingCollection), AddItemName = "Mapping")]
    public MappingCollection Mappings { get { return (MappingCollection)this["Mappings"]; } }
  }
}