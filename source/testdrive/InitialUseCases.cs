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
  interface IInfoSource { void Start(); void Stop(); string GetDispatcherInUse(); }
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
  class TopicSource : IInfoSource
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

  #region Approach 1: Replicate into separated independent classes that are used separately on the typemap.
  class TopicSourceForSpecificDispatcher1Purpose : IInfoSource
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
  class TopicSourceForOtherPurpose : IInfoSource
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
  #region Approach 2: Templated Abstract Base
  abstract class TopicSourceBase : IInfoSource
  {
    public void Start()
    {
      StartSubscription();
    }
    public void Stop()
    {
      StopSubscription();
    }
    public abstract string GetDispatcherInUse();
    protected abstract void StartSubscription();
    protected abstract void StopSubscription();
  }
  class TopicSourceWithDispatcher1 : TopicSourceBase
  {
    Dispatcher1 subscription;

    protected override void StartSubscription()
    {
      subscription = new Dispatcher1();
      subscription.Start();
    }
    protected override void StopSubscription()
    {
      subscription.Stop();
    }
    public override string GetDispatcherInUse()
    {
      return subscription.GetType().Name;
    }
  }
  class TopicSourceWithDispatcher2 : TopicSourceBase
  {
    Dispatcher2 subscription;

    protected override void StartSubscription()
    {
      subscription = new Dispatcher2();
      subscription.Start();
    }
    protected override void StopSubscription()
    {
      subscription.Stop();
    }
    public override string GetDispatcherInUse()
    {
      return subscription.GetType().Name;
    }
  }
  #endregion
  #region Approach 3: Subscriber with Start/Stop Common Interface and the Abstract TopicSource calling abstract CreateDispatcher method (which returns the Subscriber Common Interface) <- this method is implemented by two separated derived clases that are used separately on the typemap.
  #endregion
}