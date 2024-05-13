using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using Novin.Endpoints;
using Novino.Abstractions;

namespace Novin;

public static class NovinoApplicationBuilderExtensions
{
  public static INovinoBuilder AddEndpoints(this INovinoBuilder builder)
  {
    builder.Services.AddSingleton<NovinoEndpointDefinitions>(new NovinoEndpointDefinitions());
    builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.Scan(scan =>
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
      scan.FromAssemblies(assemblies)
        .AddClasses(cls =>
        {
          cls.AssignableTo(typeof(IEndpoint));
        }).AsSelf()
        .WithScopedLifetime();
    });
    
    // builder.Services.AddSingleton<IConventionalRouteBuilder, ConventionalRouteBuilder>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.Configure<NovinoEndpointsOptions>(c =>
    {
      c.RouteBuilders.Add(end =>
      {
        // end.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
        // end.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        // end.MapControllers();
      });
    });
    return builder;
  }



  public static INovinoApplication UseEndpoints(this INovinoApplication app,Action<IEndpointRouteBuilder> builder)
  {
    var option = app.ApplicationBuilder.ApplicationServices.GetRequiredService<IOptions<NovinoEndpointsOptions>>();
    
    
 
    app.ApplicationBuilder.UseEndpoints(c =>
    {
      foreach (var routeBuilder in option.Value.RouteBuilders)
      {
        routeBuilder.Invoke(c);
      }
      
      builder.Invoke(c);

      if (option.Value.EnableHealthEndpoint)
      {
        
        c.MapEndpoint<HealthEndpoint>();
      }
    });
    return app;
  }
  
  
}
