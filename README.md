# ioc
##Basic .NET runtime dependency Type-Class mapping.

`TypeClassMapper` class - Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM `IUnknown::QueryInterface` method, this class follows such design tradition and relies on basic equivalent mechanisms from .NET Framework (`System.IServiceProvider` interface).

Latest released version at: http://www.nuget.org/packages/TypeClassMapper/

Please check notes before install the latest released version: https://github.com/MarcoDorantes/ioc/blob/master/ReleaseNotes.txt

##What is TypeClassMapper?
The `TypeClassMapper` class provides a simple implementation for the .NET Framework `System.IServiceProvider` interface; namely, it implements the defined .NET mechanism for retrieving a service object; that is, an object that provides custom support to other objects.

The custom support in this case is the mapping between requested types, mainly interfaces, and their related implementation class instances. The custom support also includes instance activation, i.e., object construction.

##Key concepts
«Type» as abstract data type, interface, protocol, published contract, or application programming interface (API).

«Class» as concrete class, module with implementation details, usually hidden state and private behavior, or separate-compiled executable artifact.

«Mapper» as associative array, map, symbol table, hash table, dictionary, or associative catalog.

«Object» is an instance of a «Class».

##Why do I need TypeClassMapper?
The actual need is to properly manage the dependencies on a large-scale software design and to manage the level of technical debt of its codebase. The key goal is to prevent the ever increasing costs of a Big ball of mud anti-pattern:

http://www.laputan.org/mud/

https://en.wikipedia.org/wiki/Big_ball_of_mud

There are multiple ways to properly manage the dependencies on a large-scale software design. Among the preferred approaches are those that help to keep only the mandatory number of dependencies for a given design. `TypeClassMapper` tries to be one of those preferred approaches by relying on an already defined mechanism in the .NET Framework: the `System.IServiceProvider` interface. This way the core components in a given design do not need to add any extra dependency other than .NET Framework in order to benefit from a decoupled way of mapping their requested types to their related classes.

##How does it work?
For instance, the following `CopyProcessor` class only depends on its required abstractions and on the `System.IServiceProvider` interface (part of .NET Framework since v1.1). That is, it does not depend on concrete external implementation details:

```
namespace lib1
{
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
```
A particular instance of `TypeClassMapper` must be passed to `CopyProcessor`'s constructor based on the kind of host for a particular execution context. For example, a unit testing host, an integration testing host, the actual host at a productive enviroment, etc.

A different case, in the context of a unit test as part of a Visual Studio Test Project host:

```
[TestClass]
public class SampleCase1
{
  [TestMethod]
  public void Unit_testable()
  {
    //Arrange
    var typemap = new nutility.TypeClassMapper
    (
      Type_Object_Map: new Dictionary<Type, object>
      {
        { typeof(lib1.ISource), new SourceStub() },
        { typeof(lib1.ITarget), new TargetStub() },
        { typeof(lib1.ILogBook), new LogBookStub() }
      }
    );

    //Act
    var processor = new lib1.CopyProcessor(typemap);
    processor.Copy();

    //Assert
    //Asserts on stubs...
  }
}
```

Or, in the context of a host at a productive environment, where the type-class mappings are taken from a custom configuration section at the application configuration file:

```
  var processor = new lib1.CopyProcessor(new nutility.TypeClassMapper());
  processor.Copy();
```
##Which other use cases are supported?
The unit test cases in `TypeClassMapperSpec` Test Project could be of help to check other supported use cases. Next are a few examples:

##What if I need to map the same required type for two different classes?
Given the following two concrete classes, `module1.Source` and `module3.Source1`, the following test case shows how different requests for the same type can get different mapped classes:

```
namespace module1
{
  public class Source : app1.ISource
  {
    public string Name { get { return GetType().FullName; } }
  }
}
```

```
namespace module3
{
  public abstract class Source : app1.ISource
  {
    public abstract string Name { get; }
  }
  public class Source1 : app1.ISource
  {
    private string typemap_ctor;
    public Source1(System.IServiceProvider typemap)
    {
      this.typemap_ctor = "System.IServiceProvider";
    }
    public Source1(nutility.ITypeClassMapper typemap)
    {
      this.typemap_ctor = "nutility.ITypeClassMapper";
    }
    public Source1(nutility.TypeClassMapper typemap)
    {
      this.typemap_ctor = "nutility.TypeClassMapper";
    }
    public string Name => typemap_ctor;
  }
}
```

```
[TestMethod]
public void TheTypeForMyCase1()
{
  //Arrange
  var typemap = new nutility.TypeClassMapper
  (
    new List<nutility.Mapping<Type, Type>>
    {
      new nutility.Mapping<Type, Type> { RequiredType = typeof(app1.ISource), ClientType = typeof(ExplicitMappingCases), MappedClass = typeof(module3.Source1) },
      new nutility.Mapping<Type, Type> { RequiredType = typeof(app1.ISource), ClientType = typeof(ImplicitMappingCases), MappedClass = typeof(module1.Source) }
    }
  );

  //Act
  app1.ISource source1 = typemap.GetService<app1.ISource>(Client_Type: typeof(ExplicitMappingCases));
  app1.ISource source2 = typemap.GetService<app1.ISource>(Client_Type: typeof(ImplicitMappingCases));

  //Assert
  Assert.AreEqual<string>("nutility.TypeClassMapper", source1.Name);
  Assert.AreEqual<string>("module1.Source", source2.Name);
}
```
