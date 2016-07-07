namespace utility
{
  public class MappingCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Type", IsRequired = true)]
    public string Type { get { return (string)this["Type"]; } set { this["Type"] = value; } }

    [System.Configuration.ConfigurationProperty("Class", IsRequired = true)]
    public string Class { get { return (string)this["Class"]; } set { this["Class"] = value; } }
  }
}