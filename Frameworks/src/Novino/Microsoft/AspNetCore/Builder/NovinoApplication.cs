using Novino.Abstractions.Novino.Abstractions;
using Novino.Novino;

namespace Novino.Microsoft.AspNetCore.Builder;

public class NovinoApplication (WebApplication application)  : INovinoApplication
{
  private readonly WebApplication _webApplication = application;

  public static INovBuilder CreateBuilder()
  {
    var builder = WebApplication.CreateBuilder();
    return new NovBuilder(builder);
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
