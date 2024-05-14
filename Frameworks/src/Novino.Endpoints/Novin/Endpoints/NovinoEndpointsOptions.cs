namespace Novin.Endpoints;

public class NovinoEndpointsOptions
{
  protected internal  List<Action<IEndpointRouteBuilder>> RouteBuilders { get;  } = [];
  protected internal readonly List<(Type targetType, Delegate mapTo, Delegate mapFrom)> Conversions = [];
  public bool EnableHealthEndpoint { get; set; } = true;
  
  public void AddConversion<T>(Func<string, T> mapTo, Func<T, string> mapFrom)
  {
    Conversions.Add((typeof(T), mapTo, mapFrom));
  }
  public void AddEndpoint(Action<IEndpointRouteBuilder> builder)
  {
    RouteBuilders.Add(builder);
  }
}
