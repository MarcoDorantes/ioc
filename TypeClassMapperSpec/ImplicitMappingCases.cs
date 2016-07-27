using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeClassMapperSpec
{
  [TestClass]
  public class ImplicitMappingCases
  {
    [TestMethod]
    public void VeryBasicUseCase()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper();

      //Act
      var instance = (app1.ISource)typemap.GetService(typeof(app1.ISource));

      //Assert
      Assert.AreEqual(typeof(module1.Source).FullName, instance.Name);
    }

    [TestMethod]
    public void BadRequiredType()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "A");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Parameter cannot be null: requiredType", exception.Message);
      Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
    }

    [TestMethod]
    public void BadTypeName()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "B");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Type not found: [app1.ISource] at configured scope [B] and section [<default>]", exception.Message);
      Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void BadClassName()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "C");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Mapped class for type [app1.ISource] not found: [bad.Source, TypeClassMapperSpec] at configured scope [C] and section [<default>]", exception.Message);
      Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void BadTypeInit()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "D");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Cannot create an instance of [module2.Source] at configured scope [D] and section [<default>]. Check InnerException.", exception.Message);
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOfType(exception.InnerException, typeof(System.TypeInitializationException));
      Assert.AreEqual<string>("The type initializer for 'module2.Source' threw an exception.", exception.InnerException.Message);
    }

    [TestMethod]
    public void BadTypeLoad()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "E");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Cannot create an instance of [module3.Source] at configured scope [E] and section [<default>]. Check InnerException.", exception.Message);
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOfType(exception.InnerException, typeof(System.MissingMethodException));
      Assert.AreEqual<string>("Cannot create an abstract class.", exception.InnerException.Message);
    }

    [TestMethod]
    public void BadEmptyClassName()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(scope: "F");

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
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Mapped class for type [app1.ISource] cannot be empty: [] at configured scope [F] and section [<default>]", exception.Message);
      Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void WithTypeClassConstructor()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(section: "sectionC");

      //Act
      var instance = typemap.GetService<app1.ISource>();

      //Assert
      Assert.AreEqual(typemap.GetType().FullName, instance.Name);
    }
  }
}