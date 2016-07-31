namespace lib1
{
  public interface ISource
  {
    string Name { get; }
  }
  public interface ITarget
  {
    string Name { get; }
  }
}

namespace lib1.sample
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
    public CopyProcessor(System.IServiceProvider typemap)
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
        var result = target.Write(value);
        logger.Log($"Value #{++count} from {source.Name} to {target.Name}: {result}");
      }
    }
  }
}