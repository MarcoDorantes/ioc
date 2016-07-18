# ioc
##Basic .NET runtime dependency Type-Class mapping.

TypeClassMapper class - Given the runtime dependency management tradition of early design patterns, e.g., Microsoft COM IUnknown::QueryInterface method, this class follows such design tradition and relies on basic equivalent mechanisms from .NET Framework (System.IServiceProvider interface).

Latest released version at: http://www.nuget.org/packages/TypeClassMapper/

'Type' as abstract data type, interface, protocol, public or published contract, or application programming interface (API).

'Class' as concrete class, module, implementation, usually hidden or private programmed executable artifact.

'Mapper' as associative array, map, symbol table, hash table, or dictionary.

##Why do I need TypeClassMapper?
The `TypeClassMapper` class provides a simple implementation for the .NET Framework `System.IServiceProvider` interface; namely, it implements the defined .NET mechanism for retrieving a service object; that is, an object that provides custom support to other objects.