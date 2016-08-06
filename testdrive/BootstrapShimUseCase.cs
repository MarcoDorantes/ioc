using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testdrive
{
  class BootstrapShimUseCase
  {
    static void main()
    {
    }
    public static void Run()
    {
      try { main(); } catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}