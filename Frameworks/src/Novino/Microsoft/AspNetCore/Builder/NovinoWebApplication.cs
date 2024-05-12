using Novino;
using Novino.Abstractions;

namespace Microsoft.AspNetCore.Builder;

public class NovinoWebApplication(WebApplication application) : INovinoApplication
{
    public  static INovinoBuilder CreateBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        return new NovinoBuilder(builder);
    }

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