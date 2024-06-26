﻿using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Novin.Endpoints;
using ProducesResponseTypeMetadata = Novin.Endpoints.Summary.ProducesResponseTypeMetadata;

namespace Novin;

public static class NovinoEndpointRouteBuilderExtensions
{
  public static IEndpointRouteBuilder MapEndpointFromAssembly(this IEndpointRouteBuilder app, Type type)
  {
    app.MapEndpointFromAssemblies([type.Assembly]);
    return app;
  }

  public static IEndpointRouteBuilder MapEndpointFromAssemblies(this IEndpointRouteBuilder app, Assembly[] assemblies)
  {
    var types = assemblies.SelectMany(x => x.GetTypes()).Where(x =>
      x.IsAssignableTo(typeof(IEndpoint)) && x is { IsClass: true, IsAbstract: false });
    foreach (var type in types)
    {
      app.MapEndpoint(type);
    }

    return app;
  }

  public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder app, Type type)
  {
    var sp = app.ServiceProvider.CreateScope().ServiceProvider;
    var definitions = sp.GetRequiredService<NovinoEndpointDefinitions>();
    var handler = sp.GetRequiredService(type);
    if (handler is not IEndpoint endpointHandler)
    {
      return app;
    }

    endpointHandler.Initialize();
    definitions.Add(endpointHandler.Definition);
    var c = app.MapMethods(endpointHandler.Definition.Path, [endpointHandler.Definition.Method],
      (HttpContext ctx, [FromServices] IServiceProvider provider) =>
      {
        endpointHandler.ExecuteAsync();
      });
    
    c.WithMetadata([endpointHandler.Definition]);
    endpointHandler.Definition.UserConfigAction?.Invoke(c);
    
    return app;
  }

  public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
  {
    return app.MapEndpoint(typeof(TEndpoint));
  }
}
