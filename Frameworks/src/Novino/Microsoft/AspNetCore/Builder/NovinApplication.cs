using Novino.Abstractions.Novino.Abstractions;
using Novino.Novino;

namespace Novino.Microsoft.AspNetCore.Builder;

public class NovinApplication (WebApplication application)  : INovinApplication
{
  private readonly WebApplication _webApplication = application;

  public static INovinApplicationBuilder CreateBuilder()
  {
    var builder = WebApplication.CreateBuilder();
    return new NovinApplicationBuilder(builder);
  }


  public void Run()
  {
    _webApplication.Run();
  }

  public Task RunAsync()
  {
    return _webApplication.RunAsync();
  }
}
