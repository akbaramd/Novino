using Microsoft.AspNetCore.Builder;

namespace Novino.Test;

public class ApplicationTest
{
    private INovinoBuilder _builder;


    [SetUp]
    public void Setup()
    {
        _builder = NovinoWebApplication.CreateBuilder();
    }

    [Test]
    public void TestApplicationInitializerWork()
    {
        var test = 0;
        _builder.Initialize(c => { test++; });
        _builder.Build().Run();
        Assert.That(test, Is.EqualTo(1));
    }
}