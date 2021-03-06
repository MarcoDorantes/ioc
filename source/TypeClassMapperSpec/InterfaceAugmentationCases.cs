﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TypeClassMapperSpec
{
  [TestClass]
  public class InterfaceAugmentationCases
  {
    [TestMethod]
    public void QueryMappings()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, nutility.TypeClassID>
        {
          { typeof(app1.ISource), "module1.Source, TypeClassMapperSpec" },
        },
        new Dictionary<nutility.TypeClassID, object>
        {
          { "app1.ITarget", new object() },
          { "app1.ILog", null }
        }
      );

      //Act
      var count = typemap.Mappings.Count();
      //bool found = typemap.Mappings.Any(mapping => mapping.Key == "app1.ISource" && mapping.Value.ToString().StartsWith("module1.Source"));
      bool notfound = typemap.Mappings.Any(mapping => mapping.Key.ID == "someother.ITarget");
      string mapped_found = typemap.Mappings.FirstOrDefault(mapping => mapping.Key.ID == "app1.ISource").Value.ToString();
      nutility.TypeClassID mapped_notfound = typemap.Mappings.FirstOrDefault(mapping => mapping.Key.ID == "none.ISome").Value;
      object target = typemap.Mappings.FirstOrDefault(mapping => mapping.Key.ID == "app1.ISource").Value;
      int null_instances = typemap.Mappings.Count(mapping => mapping.Value == null);

      //Assert
      Assert.AreEqual<int>(1, count);
      Assert.AreEqual<int>(0, null_instances);
      //Assert.IsTrue(found);
      Assert.IsFalse(notfound);
      Assert.AreEqual("module1.Source, TypeClassMapperSpec", mapped_found);
      Assert.IsNull(mapped_notfound);
      Assert.IsNotNull(target);
    }

    [TestMethod]
    public void GenericExpresionForGetService()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID>
      {
        { "app1.ISource", "module1.Source, TypeClassMapperSpec" }
      });

      //Act
      app1.ISource source1 = (app1.ISource)typemap.GetService(typeof(app1.ISource));
      app1.ISource source2 = typemap.GetService<app1.ISource>();

      //Assert
      Assert.IsNotNull(source1);
      Assert.IsNotNull(source2);
      Assert.AreEqual<string>(source1.Name, source2.Name);
      Assert.AreEqual<string>("module1.Source", source2.Name);
      Assert.AreEqual<string>("module1.Source, TypeClassMapperSpec, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", source2.GetType().AssemblyQualifiedName);
    }

    [TestMethod]
    public void NameWithResetableValue()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID> { { "some-type", "some-class" } });

      //Act
      typemap.SetValue("name1", "value1");
      typemap.SetValue("name1", "value2");

      //Assert
      Assert.AreEqual<string>("value2", typemap.GetValue<string>("name1"));
    }

    [TestMethod]
    public void CheckNameDoesNotExist()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID> { { "some-type", "some-class" } });

      //Act
      bool hasName = typemap.Values.Any(n => n.Key == "name1");

      //Assert
      Assert.IsFalse(hasName);
    }

    [TestMethod]
    public void CheckNameExists()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID> { { "some-type", "some-class" } });

      //Act
      typemap.SetValue("name1", "value1");
      bool hasName = typemap.Values.Any(n => n.Key == "name1");

      //Assert
      Assert.IsTrue(hasName);
    }
  }
}