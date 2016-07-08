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
      utility.ITypeClassMapper typemap = new utility.TypeClassMapper(new Dictionary<string, object> { { "app1.ISource", "module1.Source, TypeClassMapperSpec" } });

      //Act
      var count = typemap.Mappings.Count();
      bool found = typemap.Mappings.Any(mapping => mapping.Key == "app1.ISource" && mapping.Value.ToString().StartsWith("module1.Source"));
      bool notfound = typemap.Mappings.Any(mapping => mapping.Key == "someother.ITarget");
      string mapped_found = typemap.Mappings.FirstOrDefault(mapping => mapping.Key == "app1.ISource").Value.ToString();
      string mapped_notfound = typemap.Mappings.FirstOrDefault(mapping => mapping.Key == "none.ISome").Value as string;

      //Assert
      Assert.AreEqual<int>(1, count);
      Assert.IsTrue(found);
      Assert.IsFalse(notfound);
      Assert.AreEqual("module1.Source, TypeClassMapperSpec", mapped_found);
      Assert.IsNull(mapped_notfound);
    }
  }
}