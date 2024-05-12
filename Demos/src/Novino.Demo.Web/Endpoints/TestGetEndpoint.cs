using Novin.Endpoints;

namespace Novino.Demo.Web.Endpoints;

public class TestGetEndpoint(IServiceProvider serviceProvider) : Endpoint<HealthEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    MapGet("/Test");
  }

  public override async Task HandleAsync(CancellationToken cancellationToken = default)
  {
    await SendOkResponseAsync(new HealthEndpointResponse { Status = "Online" }, cancellationToken);
  }
}

public class HealthEndpointResponse
{
  public string Status { get; set; } = default!;
}
