namespace lib3
{
  using lib1;

  internal class Source : ISource
  {
    public string Name { get { return "SourceLib3"; } }
  }
  internal class Target : ITarget
  {
    public string Name { get { return "TargetLib3"; } }
  }
}