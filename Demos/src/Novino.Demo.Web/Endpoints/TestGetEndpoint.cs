using Microsoft.AspNetCore.Mvc;
using Novin.Endpoints;

namespace Novino.Demo.Web.Endpoints;

public class TestGetEndpoint(IServiceProvider serviceProvider) : Endpoint<TestGetEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    Get("/");
  }

  protected override async Task HandleAsync(CancellationToken cancellationToken = default)
  {
    await SendResponseAsync(new TestGetEndpointResponse { Status = "Online" }, cancellationToken);
  }
}

public class TestGetEndpointResponse
{
  
  public string Status { get; set; } = default!;
}
