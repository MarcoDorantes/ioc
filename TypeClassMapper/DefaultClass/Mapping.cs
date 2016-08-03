namespace nutility
{
  /// <summary>
  /// Entry for the Type-Class Catalog.
  /// </summary>
  public class Mapping
  {
    public Mapping() { }

    public Mapping(Mapping other)
    {
      RequiredType = other?.RequiredType;
      ClientType = other?.ClientType;
      MappedClass = other?.MappedClass;
    }

    public TypeClassID RequiredType;

    /// <summary>
    /// For the case where the actual Type-Class mapping depends on the value passed by the requesting client.
    /// Relevant when the <c><![CDATA[ITypeClassMapper.GetService<T>(Type Client_Type)]]></c> or <c><![CDATA[GetService<T>(TypeClassID Client_Type)]]></c> methods get called.
    /// It could also work a la COM ProgID, but not necessarily a machine-wide ProgID, only for the very specific context of an application.
    /// </summary>
    public TypeClassID ClientType;

    public TypeClassID MappedClass;
  }

  public class Mapping2<T, V> where T : class where V : class
  {
    public Mapping2() { }

    public Mapping2(Mapping2<T, V> other)
    {
      RequiredType = other?.RequiredType;
      ClientType = other?.ClientType;
      MappedClass = other?.MappedClass;
    }

    public T RequiredType;

    /// <summary>
    /// For the case where the actual Type-Class mapping depends on the value passed by the requesting client.
    /// Relevant when the <c><![CDATA[ITypeClassMapper.GetService<T>(Type Client_Type)]]></c> or <c><![CDATA[GetService<T>(TypeClassID Client_Type)]]></c> methods get called.
    /// It could also work a la COM ProgID, but not necessarily a machine-wide ProgID, only for the very specific context of an application.
    /// </summary>
    public V ClientType;

    public T MappedClass;
  }
}