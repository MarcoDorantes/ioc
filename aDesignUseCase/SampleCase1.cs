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
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, object>
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
  }
}