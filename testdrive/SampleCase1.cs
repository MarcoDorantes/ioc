using System;

namespace samplecase
{
  public interface ISource : System.Collections.IEnumerable
  {
    string Name { get; }
  }
  public interface ITarget
  {
    string Name { get; }
    string Write(object value);
  }

  public interface ILogBook
  {
    void Log(string line);
  }

  public class CopyProcessor
  {
    private ISource source;
    private ITarget target;
    private ILogBook logger;
    public CopyProcessor(IServiceProvider typemap)
    {
      source = (ISource)typemap.GetService(typeof(ISource));
      target = (ITarget)typemap.GetService(typeof(ITarget));
      logger = (ILogBook)typemap.GetService(typeof(ILogBook));
    }
    public void Copy()
    {
      int count = 0;
      foreach (var value in source)
      {
        string result = target.Write(value);
        logger.Log($"Value#{++count} from {source.Name} to {target.Name}: {result}");
      }
    }
  }

  class SampleCase1
  {
    public static void main()
    {
      var processor = new samplecase.CopyProcessor(new nutility.TypeClassMapper());
      processor.Copy();
    }
    public static void Run()
    {
      try { main(); } catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}