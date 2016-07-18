using System;
using System.Collections.Generic;
using lib1;
using nutility;

namespace testdrive
{
  class Module
  {
    private ISource source;
    private ITarget target;
    public Module(IServiceProvider typemap)
    {
      source = (ISource)typemap.GetService(typeof(ISource));
      target = (ITarget)typemap.GetService(typeof(ITarget));
    }
    public string f()
    {
      return $"{source.Name} - {target.Name}";
    }
  }
  class InitialUseCases
  {
    static void ioc1()
    {
      var typemap = new TypeClassMapper(new Dictionary<string, object>
    {
        { "lib1.ISource", "lib2.Source, lib2" },
        { "lib1.ITarget", "lib2.Target, lib2" }
    });
      var instance = new testdrive.Module(typemap);
      Console.WriteLine(instance.f());
    }
    static void ioc2()
    {
      var instance = new testdrive.Module(new TypeClassMapper());
      Console.WriteLine(instance.f());
    }
    static void ioc3()
    {
      var instance = new testdrive.Module(new TypeClassMapper("A"));
      Console.WriteLine(instance.f());
    }
    static void ioc4()
    {
      var instance = new testdrive.Module(new TypeClassMapper("B"));
      Console.WriteLine(instance.f());
    }
    static void ioc5()
    {
      var instance = new testdrive.Module(new TypeClassMapper(null, "sectionB"));
      Console.WriteLine(instance.f());
    }
    public static void Run()
    {
      try { ioc5(); } catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}