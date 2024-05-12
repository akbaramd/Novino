namespace Novin.Endpoints;

public class EndpointConfiguration
{
  public HttpMethod HttpMethod { get; set; } = default!;

  public string Route { get; set; } = string.Empty;
}
