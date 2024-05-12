namespace Novin.Endpoints;

public class HealthEndpoint(IServiceProvider serviceProvider) : Endpoint<HealthEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    MapGet("/novino/health");
  }

  public override async Task HandleAsync(CancellationToken cancellationToken = default)
  {
    await SendOkResponseAsync(new HealthEndpointResponse { Status = "Online" }, cancellationToken);
  }
}

public class HealthEndpointResponse
{
  public string Status { get; set; } =  default!;
}
