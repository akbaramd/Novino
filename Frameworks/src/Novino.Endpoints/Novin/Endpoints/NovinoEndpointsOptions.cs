namespace Novin.Endpoints;

public class NovinoEndpointsOptions
{
  public List<Action<IEndpointRouteBuilder>> RouteBuilders { get; set; } = [];
  public bool EnableHealthEndpoint { get; set; } = true;
}
