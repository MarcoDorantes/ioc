using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#region Types & Classes used by the test cases of this TestClass.
namespace app1
{
  public interface ISource
  {
    string Name { get; }
  }
}

namespace module1
{
  public class Source : app1.ISource
  {
    public string Name { get { return GetType().FullName; } }
  }
}

namespace module2
{
  public class Source : app1.ISource
  {
    static Source()
    {
      throw new Exception(".NET type initialization problem");
    }
    public string Name { get { return GetType().FullName; } }
  }
}

namespace module3
{
  public abstract class Source : app1.ISource
  {
    public abstract string Name { get; }
  }
}
#endregion

namespace TypeClassMapperSpec
{
  [TestClass]
  public class ExplicitMappingCases
  {
    [TestMethod]
    public void VeryBasicUseCase()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "module1.Source, TypeClassMapperSpec" } });

      //Act
      var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));

      //Assert
      Assert.AreEqual(typeof(module1.Source).FullName, instance.Name);
    }

    [TestMethod]
    public void BadMappings()
    {
      //Arrange
      IDictionary<string, string> mappings = null;

      //Act
      Exception exception = null;
      try
      {
        var typemap = new utility.TypeClassMapper(typemap: mappings);
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Parameter cannot be null: typemap", exception.Message);
      Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
    }

    [TestMethod]
    public void BadRequiredType()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "bad.Source, TypeClassMapperSpec" } });

      //Act
      Exception exception = null;
      try
      {
        var instance = (app1.ISource)typemap.GetService(requiredType: null);
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Parameter cannot be null: requiredType", exception.Message);
      Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
    }

    [TestMethod]
    public void BadTypeName()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "bad.ISource", "module1.Source, TypeClassMapperSpec" } });

      //Act
      Exception exception = null;
      try
      {
        var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Type not found: [app1.ISource] at configured scope [<explicit>] and section [<explicit>]", exception.Message);
      Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void BadClassName()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "bad.Source, TypeClassMapperSpec" } });

      //Act
      Exception exception = null;
      try
      {
        var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Mapped class for type [app1.ISource] not found: [bad.Source, TypeClassMapperSpec] at configured scope [<explicit>] and section [<explicit>]", exception.Message);
      Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void BadTypeInit()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "module2.Source, TypeClassMapperSpec" } });

      //Act
      Exception exception = null;
      try
      {
        var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Cannot create an instance of [module2.Source] at configured scope [<explicit>] and section [<explicit>]. Check InnerException.", exception.Message);
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOfType(exception.InnerException, typeof(System.TypeInitializationException));
      Assert.AreEqual<string>("The type initializer for 'module2.Source' threw an exception.", exception.InnerException.Message);
    }

    [TestMethod]
    public void BadTypeLoad()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, string> { { "app1.ISource", "module3.Source, TypeClassMapperSpec" } });

      //Act
      Exception exception = null;
      try
      {
        var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(utility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Cannot create an instance of [module3.Source] at configured scope [<explicit>] and section [<explicit>]. Check InnerException.", exception.Message);
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOfType(exception.InnerException, typeof(System.MissingMethodException));
      Assert.AreEqual<string>("Cannot create an abstract class.", exception.InnerException.Message);
    }
  }
}