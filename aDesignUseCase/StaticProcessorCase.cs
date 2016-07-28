using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;

#region Types & Classes used by the test cases in this TestClass

namespace App2.Contract
{
  public class RiskRequest
  {
    public int Threshold;
  }
  public class RiskResponse
  {
    public decimal RiskAmount { get; set; }
  }
}

namespace App2.BusinessLayer
{
  public interface ITransit
  {
    decimal GetTransit();
  }
  public interface IProfile
  {
    decimal GetProfile();
    System.Data.IDataReader GetProfileTotalRisk(int threshold);
  }
  public interface IThreshold
  {
    decimal GetLimit();
    System.Data.IDataReader GetThresholdTotalRisk(int threshold);
  }

  public static class ProcessorA
  {
    public static Contract.RiskResponse GetRisk(Contract.RiskRequest request, IServiceProvider typemap)
    {
      var transit = (ITransit)typemap.GetService(typeof(ITransit));
      var profile = (IProfile)typemap.GetService(typeof(IProfile));
      var threshold = (IThreshold)typemap.GetService(typeof(IThreshold));

      var response = new Contract.RiskResponse();
      response.RiskAmount = request.Threshold > threshold.GetLimit() ? transit.GetTransit() + profile.GetProfile() : 0;
      return response;
    }

    public static Contract.RiskResponse GetTotalRisk(Contract.RiskRequest request, IServiceProvider typemap)
    {
      var profile = (IProfile)typemap.GetService(typeof(IProfile));
      var threshold = (IThreshold)typemap.GetService(typeof(IThreshold));

      decimal profile_amount = 0;
      using (System.Data.IDataReader reader = profile.GetProfileTotalRisk(request.Threshold))
      {
        if(reader.Read())
        {
          profile_amount = Convert.ToDecimal(reader["Field1"]) + Convert.ToDecimal(reader["Field2"]);
        }
      }

      decimal threshold_amount = 0;
      using (System.Data.IDataReader reader = threshold.GetThresholdTotalRisk(request.Threshold))
      {
        if (reader.Read())
        {
          threshold_amount = Convert.ToDecimal(reader["Field3"]) + Convert.ToDecimal(reader["Field4"]);
        }
      }

      var response = new Contract.RiskResponse();
      response.RiskAmount = profile_amount + threshold_amount;
      return response;
    }

  }
}
namespace App2.DataAccess
{
  public class Transit : BusinessLayer.ITransit
  {
    private decimal transit;
    public Transit() : this(transit: 1) { }
    public Transit(decimal transit)
    {
      this.transit = transit;
    }
    public decimal GetTransit() => transit;
  }
  public class Profile : BusinessLayer.IProfile, System.Data.IDataReader
  {
    private decimal profile;
    public Profile() : this(profile: 2) { }
    public Profile(decimal profile)
    {
      this.profile = profile;
    }
    public decimal GetProfile() => profile;
    public System.Data.IDataReader GetProfileTotalRisk(int threshold)
    {
      return this;
    }

    #region IDataReader
    public object this[string name]
    {
      get
      {
        switch (name)
        {
          case "Field1": return 100;
          case "Field2": return 200;
          default:
            throw new Exception($"this[string] called for unexpected field: [{name}]");
        }
      }
    }

    public object this[int i]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int Depth
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int FieldCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool IsClosed
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int RecordsAffected
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void Close() {}

    public void Dispose(){}

    public bool GetBoolean(int i)
    {
      throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
      throw new NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
      throw new NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
      throw new NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
      throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
      throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
      throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
      throw new NotImplementedException();
    }

    public Type GetFieldType(int i)
    {
      throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
      throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
      throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
      throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
      throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
      throw new NotImplementedException();
    }

    public string GetName(int i)
    {
      throw new NotImplementedException();
    }

    public int GetOrdinal(string name)
    {
      throw new NotImplementedException();
    }

    public DataTable GetSchemaTable()
    {
      throw new NotImplementedException();
    }

    public string GetString(int i)
    {
      throw new NotImplementedException();
    }

    public object GetValue(int i)
    {
      throw new NotImplementedException();
    }

    public int GetValues(object[] values)
    {
      throw new NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
      throw new NotImplementedException();
    }

    public bool NextResult()
    {
      throw new NotImplementedException();
    }

    public bool Read()
    {
      return true;
    }
    #endregion
  }
  public class Threshold : BusinessLayer.IThreshold
  {
    internal System.Data.IDataReader data;
    private decimal threshold;
    public Threshold() : this(threshold: 101) { }
    public Threshold(decimal threshold)
    {
      this.threshold = threshold;
    }
    public decimal GetLimit() => threshold;
    public System.Data.IDataReader GetThresholdTotalRisk(int threshold)
    {
      return data;
    }
  }
  static class Factory1
  {
    public static object CreateInstance() => new App2.DataAccess.Transit(100);
  }
  class Factory2
  {
    public object CreateInstance() => new App2.DataAccess.Threshold(500);
  }
  class TestDataReader : IDataReader
  {
    IDictionary<string, object> data;
    public TestDataReader(IDictionary<string, object> data)
    {
      this.data = data;
    }
    public object this[string name]
    {
      get
      {
        return data[name];
      }
    }

    public object this[int i]
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int Depth
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int FieldCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool IsClosed
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int RecordsAffected
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void Close(){}

    public void Dispose(){}

    public bool GetBoolean(int i)
    {
      throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
      throw new NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
      throw new NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
      throw new NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
      throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
      throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
      throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
      throw new NotImplementedException();
    }

    public Type GetFieldType(int i)
    {
      throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
      throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
      throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
      throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
      throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
      throw new NotImplementedException();
    }

    public string GetName(int i)
    {
      throw new NotImplementedException();
    }

    public int GetOrdinal(string name)
    {
      throw new NotImplementedException();
    }

    public DataTable GetSchemaTable()
    {
      throw new NotImplementedException();
    }

    public string GetString(int i)
    {
      throw new NotImplementedException();
    }

    public object GetValue(int i)
    {
      throw new NotImplementedException();
    }

    public int GetValues(object[] values)
    {
      throw new NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
      throw new NotImplementedException();
    }

    public bool NextResult()
    {
      throw new NotImplementedException();
    }

    public bool Read()
    {
      return true;
    }
  }
}

#endregion

namespace aDesignUseCase
{
  [TestClass]
  public class StaticProcessorCase
  {
    [TestMethod]
    public void ProcessorStaticOperation1_A()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<string, nutility.TypeClassID>
      {
        { "App2.BusinessLayer.ITransit", "App2.DataAccess.Transit, aDesignUseCase" },
        { "App2.BusinessLayer.IProfile", "App2.DataAccess.Profile, aDesignUseCase" },
        { "App2.BusinessLayer.IThreshold", "App2.DataAccess.Threshold, aDesignUseCase" }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 105 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(3, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1_B()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, nutility.TypeClassID>
      {
        { typeof(App2.BusinessLayer.ITransit), typeof(App2.DataAccess.Transit).AssemblyQualifiedName },
        { typeof(App2.BusinessLayer.IProfile), "App2.DataAccess.Profile, aDesignUseCase" },
        { typeof(App2.BusinessLayer.IThreshold), "App2.DataAccess.Threshold, aDesignUseCase" }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 105 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(3, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1_C()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(App2.BusinessLayer.ITransit), typeof(App2.DataAccess.Transit) },
        { typeof(App2.BusinessLayer.IProfile), typeof(App2.DataAccess.Profile) },
        { typeof(App2.BusinessLayer.IThreshold), typeof(App2.DataAccess.Threshold) }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 105 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(3, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation2_B()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(
        new Dictionary<Type, nutility.TypeClassID>
        {
          { typeof(App2.BusinessLayer.IProfile), "App2.DataAccess.Profile, aDesignUseCase" }
        },
        new Dictionary<Type, object>
        {
          { typeof(App2.BusinessLayer.IThreshold), new App2.DataAccess.Threshold() { data = new App2.DataAccess.TestDataReader(new Dictionary<string, object> { { "Field3", 200M }, { "Field4", 300M } }) } }
        }
      );
      var request = new App2.Contract.RiskRequest() { Threshold = 105 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetTotalRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(800, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1WithInstances_A()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper
      (
        new Dictionary<Type, Type>
        {
          { typeof(App2.BusinessLayer.IProfile), typeof(App2.DataAccess.Profile) }
        },
        new Dictionary<Type, object>
        {
          { typeof(App2.BusinessLayer.ITransit), new App2.DataAccess.Transit(100) },
          { typeof(App2.BusinessLayer.IThreshold), new App2.DataAccess.Threshold(500) }
        }
      );
      var request = new App2.Contract.RiskRequest() { Threshold = 501 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(102, response.RiskAmount);
    }

    /*[TestMethod]
    public void ProcessorStaticOperation1WithInstances_B()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, object>
      {
        { typeof(App2.BusinessLayer.ITransit), new App2.DataAccess.Transit(100) },
        { typeof(App2.BusinessLayer.IProfile), "App2.DataAccess.Profile, aDesignUseCase" },
        { typeof(App2.BusinessLayer.IThreshold), new App2.DataAccess.Threshold(500) }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 501 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(102, response.RiskAmount);
    }*/

    [TestMethod]
    public void ProcessorStaticOperation1WithInstances_C()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(App2.BusinessLayer.IProfile), typeof(App2.DataAccess.Profile) }
      },
      new Dictionary<Type, Func<object>>
      {
        { typeof(App2.BusinessLayer.ITransit), new Func<object>(App2.DataAccess.Factory1.CreateInstance) },
        { typeof(App2.BusinessLayer.IThreshold), new Func<object>((new App2.DataAccess.Factory2()).CreateInstance) }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 501 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(102, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1WithInstances_D()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(App2.BusinessLayer.IProfile), typeof(App2.DataAccess.Profile) }
      },
      new Dictionary<string, Func<object>>
      {
        { "App2.BusinessLayer.ITransit", new Func<object>(App2.DataAccess.Factory1.CreateInstance) },
        { "App2.BusinessLayer.IThreshold", new Func<object>((new App2.DataAccess.Factory2()).CreateInstance) }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 501 };

      //Act
      App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(102, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1WithInstances_D_WithException()
    {
      //Arrange
      var typemap = new nutility.TypeClassMapper(new Dictionary<Type, Type>
      {
        { typeof(App2.BusinessLayer.IProfile), typeof(App2.DataAccess.Profile) }
      },
      new Dictionary<string, Func<object>>
      {
        { "App2.BusinessLayer.ITransit", new Func<object>(()=> { throw new Exception("creation exception"); } ) },
        { "App2.BusinessLayer.IThreshold", new Func<object>((new App2.DataAccess.Factory2()).CreateInstance) }
      });
      var request = new App2.Contract.RiskRequest() { Threshold = 501 };

      //Act
      Exception exception = null;
      try
      {
        App2.Contract.RiskResponse response = App2.BusinessLayer.ProcessorA.GetRisk(request, typemap);
      }
      catch (Exception ex)
      {
        exception = ex;
      }

      //Assert
      Assert.IsNotNull(exception);
      Assert.AreEqual<Type>(typeof(nutility.TypeClassMapperException), exception.GetType());
      Assert.AreEqual<string>("Class creator throws for Type [App2.BusinessLayer.ITransit] at configured scope [<explicit>] and section [<explicit>]. Check InnerException.", exception.Message);
      Assert.IsNotNull(exception.InnerException);
      Assert.IsInstanceOfType(exception.InnerException, typeof(System.Exception));
      Assert.AreEqual<string>("creation exception", exception.InnerException.Message);
    }
  }
}