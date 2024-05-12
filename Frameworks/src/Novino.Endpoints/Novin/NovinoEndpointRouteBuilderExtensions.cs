using Novin.Endpoints;

namespace Novin;

public static class NovinoEndpointRouteBuilderExtensions
{
  public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
  {
    var handler = app.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<TEndpoint>();
    handler.Configure();
    app.MapMethods(handler.Pattern, [handler.HttpMethod], _ => handler.ExecuteAsync());
    return app;
  }
}
