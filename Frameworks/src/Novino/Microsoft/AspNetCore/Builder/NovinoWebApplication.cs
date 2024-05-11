using Novino;
using Novino.Abstractions;

namespace Microsoft.AspNetCore.Builder;

public class NovinoWebApplication(WebApplication application) : INovinoApplication
{
    public new static INovinoApplicationBuilder CreateBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        return new NovinoApplicationBuilder(builder);
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