using System;
using System.Collections;
using System.Collections.Generic;

namespace samplecase
{
  class ConsoleSourceTargetLogBook : lib1.sample.ISource, lib1.sample.ITarget, lib1.sample.ILogBook, IEnumerator
  {
    private string current;

    public string Name => nameof(ConsoleSourceTargetLogBook);

    #region IEnumerator
    public object Current => current;

    public bool MoveNext()
    {
      current = Console.ReadLine();
      return string.Compare(current, "quit", true) != 0;
    }

    public void Reset()
    {
      current="";
    }
    #endregion

    IEnumerator IEnumerable.GetEnumerator() => this;

    public string Write(object value)
    {
      Console.WriteLine(value);
      return "ACK";
    }
    public void Log(string line)
    {
      Console.WriteLine(line);
    }
  }

  class SampleCase1
  {
    public static void main()
    {
      nutility.ITypeClassMapper typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(lib1.sample.ISource), typeof(samplecase.ConsoleSourceTargetLogBook) },
        { typeof(lib1.sample.ITarget), typeof(samplecase.ConsoleSourceTargetLogBook) },
        { typeof(lib1.sample.ILogBook), typeof(samplecase.ConsoleSourceTargetLogBook) }
      });
      var processor = new lib1.sample.CopyProcessor(typemap);
      processor.Copy();
    }
    public static void Run()
    {
      try { main(); } catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}