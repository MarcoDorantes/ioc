using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aDesignUseCase
{
  #region Test classes for SampleCase1
  namespace libx
  {
    class SourceStub : lib1.sample.ISource
    {
      private string name;
      private IEnumerable items;
      public SourceStub(string name, IEnumerable items)
      {
        this.name = name;
        this.items = items;
      }
      public string Name => name;

      IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }

    class TargetStub : lib1.sample.ITarget
    {
      private string name;
      public List<object> values;
      public TargetStub(string name)
      {
        this.name = name;
        values = new List<object>();
      }
      public string Name => name;

      public string Write(object value)
      {
        values.Add(value);
        return "ACK";
      }

      public override string ToString() => name;
    }
    class LogBookStub : lib1.sample.ILogBook
    {
      public List<string> lines;
      public LogBookStub()
      {
        lines = new List<string>();
      }
      public void Log(string line)
      {
        lines.Add(line);
      }
    }
  }
  namespace liby
  {
    class A : lib1.ISource
    {
      public A(nutility.ITypeClassMapper typemap)
      {
        ID = typemap.GetValue<int>(nameof(ID));
        Name = typemap.GetValue<string>(nameof(Name));
      }
      public int ID { get; private set; }
      public string Name { get; private set; }
    }
    class B : lib1.ISource
    {
      public B(nutility.ITypeClassMapper typemap)
      {
        ID = typemap.GetService<int>();
        Name = typemap.GetService<string>();
      }
      public int ID { get; private set; }
      public string Name { get; private set; }
    }
    class C : lib1.ITarget
    {
      public C(nutility.ITypeClassMapper typemap)
      {
        Name = typemap.GetService<string>();
      }
      public string Name { get; set; }
    }
  }
  #endregion

  [TestClass]
  public class SampleCase1
  {
    [TestMethod]
    public void Unit_testable()
    {
      //Arrange
      var source_stub = new libx.SourceStub("source1", new string[] { "one", "two" });
      var target_stub = new libx.TargetStub("target1");
      var log_stub = new libx.LogBookStub();
      var typemap = new nutility.TypeClassMapper(Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.sample.ISource), source_stub },
        { typeof(lib1.sample.ITarget), target_stub },
        { typeof(lib1.sample.ILogBook), log_stub }
      });

      //Act
      var processor = new lib1.sample.CopyProcessor(typemap);
      processor.Copy();

      //Assert
      Assert.AreEqual<int>(2, target_stub.values.Count);
      Assert.IsTrue(log_stub.lines.All(line => line.EndsWith("ACK")));
    }

    [TestMethod]
    public void ObjectMapDuplicate_Tolerable()
    {
      //Arrange
      var source_stub = new libx.SourceStub("source1", new string[] { "one", "two" });
      var target_stub = new libx.TargetStub("target1");
      var log_stub = new libx.LogBookStub();
      var typemap = new nutility.TypeClassMapper(Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.sample.ISource), source_stub },
        { typeof(lib1.sample.ITarget), target_stub },
        { typeof(lib1.sample.ILogBook), log_stub }
      });

      //Act
      typemap.AddMapping<lib1.sample.ITarget>(target_stub);
      var processor = new lib1.sample.CopyProcessor(typemap);
      processor.Copy();

      //Assert
      Assert.AreEqual<int>(2, target_stub.values.Count);
      Assert.IsTrue(log_stub.lines.All(line => line.EndsWith("ACK")));
    }

    [TestMethod]
    public void ObjectMapDuplicate_Tolerable2()
    {
      //Arrange
      var source_stub = new libx.SourceStub("source1", new string[] { "one", "two" });
      var log_stub = new libx.LogBookStub();
      var typemap = new nutility.TypeClassMapper(Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.sample.ISource), source_stub },
        { typeof(lib1.sample.ITarget), null },
        { typeof(lib1.sample.ILogBook), log_stub }
      });

      //Act
      typemap.AddMapping<lib1.sample.ITarget>(null);
      var target = (lib1.sample.ITarget)typemap.GetService(typeof(lib1.sample.ITarget));

      //Assert
      Assert.IsNull(target);
    }

    [TestMethod]
    public void ObjectMapDuplicate_NonTolerable()
    {
      //Arrange
      var source_stub = new libx.SourceStub("source1", new string[] { "one", "two" });
      var target_stub = new libx.TargetStub("target1");
      var targe_stub2 = new libx.TargetStub("target2");
      var log_stub = new libx.LogBookStub();
      var typemap = new nutility.TypeClassMapper(Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.sample.ISource), source_stub },
        { typeof(lib1.sample.ITarget), target_stub },
        { typeof(lib1.sample.ILogBook), log_stub }
      });
      nutility.TypeClassMapperException expected_exception = null;

      //Act
      try
      {
        typemap.AddMapping<lib1.sample.ITarget>(targe_stub2);
      }
      catch (nutility.TypeClassMapperException ex) { expected_exception = ex; }

      //Assert
      Assert.IsNotNull(expected_exception);
      Assert.AreEqual("Unsupported non-deterministic object mapping (target1:aDesignUseCase.libx.TargetStub, target2:aDesignUseCase.libx.TargetStub) for lib1.sample.ITarget.", expected_exception.Message);
    }

    [TestMethod]
    public void ObjectMapDuplicate_NonTolerable2()
    {
      //Arrange
      var source_stub = new libx.SourceStub("source1", new string[] { "one", "two" });
      var target_stub = new libx.TargetStub("target1");
      var log_stub = new libx.LogBookStub();
      var typemap = new nutility.TypeClassMapper(Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.sample.ISource), source_stub },
        { typeof(lib1.sample.ITarget), target_stub },
        { typeof(lib1.sample.ILogBook), log_stub }
      });
      nutility.TypeClassMapperException expected_exception = null;

      //Act
      try
      {
        typemap.AddMapping<lib1.sample.ITarget>(null);
      }
      catch (nutility.TypeClassMapperException ex) { expected_exception = ex; }

      //Assert
      Assert.IsNotNull(expected_exception);
      Assert.AreEqual("Unsupported non-deterministic object mapping (target1:aDesignUseCase.libx.TargetStub, <null>) for lib1.sample.ITarget.", expected_exception.Message);
    }

    [TestMethod]
    public void Get_values1()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(lib1.ISource), typeof(liby.A) }
      },
      new Dictionary<string, object>
      {
        { "ID", 123 },
        { "Name", "name123" }
      });

      //Act
      var a = typemap.GetService<lib1.ISource>();

      //Assert
      Assert.AreEqual<string>("name123", a.Name);
      Assert.AreEqual<int>(123, ((liby.A)a).ID);
    }

    [TestMethod]
    public void Get_values2()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(lib1.ISource), typeof(liby.A) }
      });
      typemap.SetValue<int>("ID", 123);
      typemap.SetValue<string>("Name", "name123");

      //Act
      var a = typemap.GetService<lib1.ISource>();

      //Assert
      Assert.AreEqual<string>("name123", a.Name);
      Assert.AreEqual<int>(123, ((liby.A)a).ID);
    }

    [TestMethod]
    public void AddMappingsAndGetAsValues()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(lib1.ISource), typeof(liby.B) }
        }
      );
      typemap.AddMapping<int>(123);
      typemap.AddMapping<string>("name123");
      typemap.AddMapping(new nutility.Mapping<nutility.TypeClassID, nutility.TypeClassID> { RequiredType = "lib1.ITarget", MappedClass = "aDesignUseCase.liby.C, aDesignUseCase" });

      //Act
      var b = typemap.GetService<lib1.ISource>();
      var s = typemap.GetService<lib1.ITarget>();

      //Assert
      Assert.AreEqual<string>("name123", b.Name);
      Assert.AreEqual<int>(123, ((liby.B)b).ID);
      Assert.AreEqual<string>("name123", s.Name);
    }

    [TestMethod]
    public void AddMappingsAndGetAsValues2()
    {
      //Arrange
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(lib1.ISource), typeof(liby.B) }
        }
      );
      typemap.AddMapping<int>(321);
      typemap.AddMapping<string>(null);

      //Act
      var b = typemap.GetService<lib1.ISource>();

      //Assert
      Assert.AreEqual<string>(null, b.Name);
      Assert.AreEqual<int>(321, ((liby.B)b).ID);
    }
  }
}