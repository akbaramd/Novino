using Microsoft.AspNetCore.Builder;

namespace Novino.Test;

public class ApplicationTest
{
    private INovinoApplicationBuilder _applicationBuilder;


    [SetUp]
    public void Setup()
    {
        _applicationBuilder = NovinoWebApplication.CreateBuilder();
    }

    [Test]
    public void TestApplicationInitializerWork()
    {
        var test = 0;
        _applicationBuilder.AddBuildAction(c => { test++; });
        _applicationBuilder.Build().Run();
        Assert.That(test, Is.EqualTo(1));
    }
}