using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#region Types & Classes used by the test cases in this TestClass

namespace app1.Contract
{
  public class RiskRequest
  {
    public int Threshold;
  }
  public class RiskResponse
  {
    public decimal RiskAmount;
  }
}

namespace app1.BusinessLayer
{
  public interface ITransit
  {
    decimal GetTransit();
  }
  public interface IProfile
  {
    decimal GetProfile();
  }
  public interface IThreshold
  {
    decimal GetLimit();
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
  }
}
namespace app1.DataAccess
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
  public class Profile : BusinessLayer.IProfile
  {
    private decimal profile;
    public Profile() : this(profile: 2) { }
    public Profile(decimal profile)
    {
      this.profile = profile;
    }
    public decimal GetProfile() => profile;
  }
  public class Threshold : BusinessLayer.IThreshold
  {
    private decimal threshold;
    public Threshold() : this(threshold: 101) { }
    public Threshold(decimal threshold = 101)
    {
      this.threshold = threshold;
    }
    public decimal GetLimit() => threshold;
  }
}

#endregion

namespace aDesignUseCase
{
  [TestClass]
  public class ProcessorCase
  {
    [TestMethod]
    public void ProcessorStaticOperation1()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, object>
      {
        { "app1.BusinessLayer.ITransit", "app1.DataAccess.Transit, aDesignUseCase" },
        { "app1.BusinessLayer.IProfile", "app1.DataAccess.Profile, aDesignUseCase" },
        { "app1.BusinessLayer.IThreshold", "app1.DataAccess.Threshold, aDesignUseCase" }
      });
      var request = new app1.Contract.RiskRequest() { Threshold = 105 };

      //Act
      app1.Contract.RiskResponse response = app1.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(3, response.RiskAmount);
    }

    [TestMethod]
    public void ProcessorStaticOperation1WithInstances()
    {
      //Arrange
      var typemap = new utility.TypeClassMapper(new Dictionary<string, object>
      {
        { "app1.BusinessLayer.ITransit", new app1.DataAccess.Transit(100) },
        { "app1.BusinessLayer.IProfile", "app1.DataAccess.Profile, aDesignUseCase" },
        { "app1.BusinessLayer.IThreshold", new app1.DataAccess.Threshold(500) }
      });
      var request = new app1.Contract.RiskRequest() { Threshold = 501 };

      //Act
      app1.Contract.RiskResponse response = app1.BusinessLayer.ProcessorA.GetRisk(request, typemap);

      //Assert
      Assert.AreEqual<decimal>(102, response.RiskAmount);
    }
  }
}