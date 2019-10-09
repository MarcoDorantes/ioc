using System;
using System.IO;

using static System.Console;

namespace impl
{
  public class Handler : app1.IHandler
  {
    public void Show(TextWriter w)
    {
      w.WriteLine(GetType().FullName);
    }
  }
}

namespace app1
{
  public interface IHandler
  {
    void Show(TextWriter w);
  }
  class Program
  {
    static IHandler create(IServiceProvider typemap) => typemap.GetService(typeof(IHandler)) as IHandler;
    static void Main(string[] args)
    {
      //WriteLine($"Config:{ConfigurationManager.AppSettings["A"]}");
      WriteLine("Start");
      IHandler h = create(new nutility.TypeClassMapper());
      h.Show(Out);
    }
  }
}