# ioc
##Basic .NET runtime dependency Type-Class mapping.

`TypeClassMapper` class - Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM `IUnknown::QueryInterface` method, this class follows such design tradition and relies on basic equivalent mechanisms from .NET Framework (`System.IServiceProvider` interface).

Latest released version at: http://www.nuget.org/packages/TypeClassMapper/

##What is TypeClassMapper?
The `TypeClassMapper` class provides a simple implementation for the .NET Framework `System.IServiceProvider` interface; namely, it implements the defined .NET mechanism for retrieving a service object; that is, an object that provides custom support to other objects.

The custom support in this case is the mapping between requested types, mainly interfaces, and their related implementation class instances. The custom support also includes instance activation, i.e., object construction.

##Key concepts
«Type» as abstract data type, interface, protocol, public or published contract, or application programming interface (API).

«Class» as concrete class, module, implementation, usually hidden or private programmed executable artifact.

«Mapper» as associative array, map, symbol table, hash table, or dictionary.

##Why do I need TypeClassMapper?
The actual need is to properly manage the dependencies on a large-scale software design and to manage the level of technical debt of its codebase. The key goal is to prevent the ever increasing costs of a Big ball of mud anti-pattern:

http://www.laputan.org/mud/

https://en.wikipedia.org/wiki/Big_ball_of_mud

There are multiple ways to properly manage the dependencies on a large-scale software design. Among the preferred approaches are those that help to keep only the mandatory number of dependencies for a given design. `TypeClassMapper` tries to be one of those preferred approaches by relying on an already defined mechanism in the .NET Framework: the `System.IServiceProvider` interface. This way the core components in a given design do not need to add any extra dependency other than .NET Framework in order to benefit from a decoupled way of mapping their requested types to their related classes.

##How does it work?
For example, the following `CopyProcessor` class only depends on its required abstractions and on the `System.IServiceProvider` interface. That is, it does not depend on concrete implementation details:

```
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
        logger.Log($"Value #{++count} from {source.Name} to {target.Name}: {result}");
      }
    }
  }
```
A particular instance of `TypeClassMapper` must be passed to `CopyProcessor`'s constructor based on the kind of host for a particular execution context. For example, a unit testing host, an integration testing host, the actual host at a productive enviroment, etc.