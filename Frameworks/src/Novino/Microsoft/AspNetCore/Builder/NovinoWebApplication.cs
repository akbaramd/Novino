using Novino;
using Novino.Abstractions;

namespace Microsoft.AspNetCore.Builder;

public class NovinoWebApplication(WebApplication application, NovinoServiceOptions serviceOptions) : INovinoApplication
{
    public  static INovinoBuilder CreateBuilder(string id , string name,string version = "1.0.0")
    {
        var builder = WebApplication.CreateBuilder();
        return new NovinoBuilder(builder,new NovinoServiceOptions()
        {
          Id = id.ToKebabCase(),
          Name = name,
          Version = version
        });
    }

    public NovinoServiceOptions ServiceOptions { get; } = serviceOptions;
    public IApplicationBuilder ApplicationBuilder => application;

    public void Run()
    {
        RunAsync().GetAwaiter().GetResult();
    }

    public  async Task RunAsync()
    {
        await application.RunAsync();
    }


    public async ValueTask DisposeAsync()
    {
        await application.DisposeAsync();
    }
}