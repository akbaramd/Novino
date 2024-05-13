namespace Novin.Endpoints;

public class HealthEndpoint(IServiceProvider serviceProvider) : Endpoint<HealthEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    Get("/novino/health");
    
  }

  
  public override async Task HandleAsync(CancellationToken cancellationToken = default)
  {
    await SendResponseAsync(new HealthEndpointResponse { Status = "Online" }, cancellationToken);
  }
}

public class HealthEndpointResponse
{
  public string Status { get; set; } =  default!;
}
