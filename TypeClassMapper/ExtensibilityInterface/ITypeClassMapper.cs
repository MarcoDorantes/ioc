using System;
using System.Collections.Generic;

namespace nutility
{
  /// <summary>
  /// By now, if query to existing mappings is required, then casting to this interface is in order.
  /// Also, 
  /// </summary>
  public interface ITypeClassMapper : IServiceProvider
  {
    /// <summary>
    /// Existing type-class mappings.
    /// </summary>
    IEnumerable<KeyValuePair<string, object>> Mappings { get; }

    /// <summary>
    /// For a given tradeoff between little syntactic sugar and the dependency to this ITypeClassMapper interface, this method would be the generic version of System.IServiceProvider.GetService method.
    /// </summary>
    /// <typeparam name="T">Required Type</typeparam>
    /// <returns>Mapped Class</returns>
    T GetService<T>();

    void SetValue<T>(string name, T value);
    T GetValue<T>(string name);
  }
}