namespace lib3
{
  using System;
  using lib1;

  internal class Source : ISource
  {
    public string Name { get { return "SourceLib3"; } }
  }
  internal class Target : ITarget
  {
    public string Name { get { return "TargetLib3"; } }
  }

  public class TargetCtorException : ITarget
  {
    public TargetCtorException()
    {
      throw new ArgumentNullException("noparam");
    }
    string ITarget.Name
    {
      get
      {
        throw new NotImplementedException();
      }
    }
  }
}