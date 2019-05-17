namespace nutility
{
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
}