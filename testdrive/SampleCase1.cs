using System;

namespace samplecase
{
  class SampleCase1
  {
    public static void main()
    {
      var processor = new lib1.sample.CopyProcessor(new nutility.TypeClassMapper());
      processor.Copy();
    }
    public static void Run()
    {
      try { main(); } catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}