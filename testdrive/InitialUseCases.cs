using System;
using System.Collections.Generic;
using lib1;
using nutility;
using static System.Console;

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
      return $"{source.Name} <-> {target.Name}";
    }
  }
  class InitialUseCases
  {
    static void ioc1()
    {
      var typemap = new TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID>
      {
          { "lib1.ISource", "lib2.Source, lib2" },
          { "lib1.ITarget", "lib2.Target, lib2" }
      });
      var instance = new testdrive.Module(typemap);
      WriteLine(instance.f());
    }
    static void ioc2()
    {
      var instance = new testdrive.Module(new TypeClassMapper());
      WriteLine(instance.f());
    }
    static void ioc3()
    {
      var instance = new testdrive.Module(new TypeClassMapper("A"));
      WriteLine(instance.f());
    }
    static void ioc4()
    {
      var instance = new testdrive.Module(new TypeClassMapper("B"));
      WriteLine(instance.f());
    }
    static void ioc5()
    {
      var instance = new testdrive.Module(new TypeClassMapper(null, "sectionB"));
      WriteLine(instance.f());
    }
    static void ioc6()
    {
      try
      {
        var typemap = new TypeClassMapper(new Dictionary<nutility.TypeClassID, nutility.TypeClassID>
      {
          { "lib1.ISource", "lib2.Source, lib2" },
          { "lib1.ITarget", "lib3.TargetCtorException, lib3" }
      });
        var instance = new testdrive.Module(typemap);
        WriteLine("problem; no exception");
      }
      catch (nutility.TypeClassMapperException exception)
      {
        var logline = new System.Text.StringBuilder($"{exception.Message}");
        int level = -1;
        Exception ex = exception.InnerException;
        do
        {
          logline.Append($"\n[InnerException Level {++level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
          ex = ex.InnerException;
        } while (ex != null);
        WriteLine($"\nNotify: {logline}");
      }
    }
    public static void Run()
    {
      try { ioc6(); } catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }

  #region Fixed
  interface ITopicSource { void Start(); void Stop(); string GetDispatcherInUse(); }
  class Dispatcher1
  {
    public void Start()
    {
      WriteLine($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
    }
    public void Stop()
    {
      WriteLine($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
    }
  }
  class Dispatcher2
  {
    public void Start()
    {
      WriteLine($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
    }
    public void Stop()
    {
      WriteLine($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
    }
  }
  #endregion

  #region Situation
  class TopicSource : ITopicSource
  {
    Dispatcher1 subscription;
    //Dispatcher2 subscription;

    public void Start()
    {
      subscription.Start();
    }
    public void Stop()
    {
      subscription.Stop();
    }
    public string GetDispatcherInUse()
    {
      subscription = new Dispatcher1();
      //subscription = new Dispatcher2();

      return subscription.GetType().Name;
    }
  }
  #endregion

  #region Approach 1
  class TopicSourceForSpecificDispatcher1Purpose : ITopicSource
  {
    Dispatcher1 subscription;

    public void Start()
    {
      subscription.Start();
    }
    public void Stop()
    {
      subscription.Stop();
    }
    public string GetDispatcherInUse()
    {
      subscription = new Dispatcher1();
      return subscription.GetType().Name;
    }
  }
  class TopicSourceForOtherPurpose : ITopicSource
  {
    Dispatcher2 subscription;

    public void Start()
    {
      subscription.Start();
    }
    public void Stop()
    {
      subscription.Stop();
    }
    public string GetDispatcherInUse()
    {
      subscription = new Dispatcher2();

      return subscription.GetType().Name;
    }
  }
  #endregion
}