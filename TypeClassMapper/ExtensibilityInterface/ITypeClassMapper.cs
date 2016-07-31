using System;
using System.Collections.Generic;

namespace nutility
{
  /// <summary>
  /// By now, if query to existing mappings is required, or other extended cases are required, then casting to this interface is in order.
  /// </summary>
  public interface ITypeClassMapper : IServiceProvider
  {
    /// <summary>
    /// A copy of the existing type-class mappings.
    /// </summary>
    IEnumerable<KeyValuePair<TypeClassID, TypeClassID>> Mappings { get; }

    /// <summary>
    /// A copy of the existing type-class catalog.
    /// </summary>
    IEnumerable<Mapping> Catalog { get; }

    /// <summary>
    /// A copy of the existing type-object values.
    /// </summary>
    IEnumerable<KeyValuePair<string, object>> Values { get; }

    /// <summary>
    /// For a given tradeoff between little syntactic sugar and the dependency to this ITypeClassMapper interface, this method would be the generic version of System.IServiceProvider.GetService method.
    /// </summary>
    /// <typeparam name="T">Required Type</typeparam>
    /// <returns>Mapped Class</returns>
    T GetService<T>();

    T GetService<T>(Type Client_Type);

    T GetService<T>(TypeClassID Client_Type);

    void AddMapping<T>(T value);

    void SetValue<T>(string name, T value);

    T GetValue<T>(string name);
  }
}