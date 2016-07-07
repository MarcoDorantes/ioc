using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TypeClassMapperSpec
{
  [TestClass]
  public class QueryMappingCases
  {
    [TestMethod]
    public void QueryMappings()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "module1.Source, TypeClassMapperSpec" } });

      //Act
      var count = typemap.Mappings.Count();
      bool found = typemap.Mappings.Any(mapping => mapping.Key == "app1.ISource" && mapping.Value.StartsWith("module1.Source"));

      //Assert
      Assert.AreEqual<int>(1, count);
      Assert.IsTrue(found);
    }
  }
}