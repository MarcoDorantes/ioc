namespace nutility
{
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
}