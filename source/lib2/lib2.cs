namespace lib2
{
  using lib1;

  internal class Source : ISource
  {
    public string Name { get { return GetType().AssemblyQualifiedName; } }
  }
  internal class Target : ITarget
  {
    public string Name { get { return GetType().AssemblyQualifiedName; ; } }
  }
}