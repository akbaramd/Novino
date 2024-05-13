using Novino.Abstractions;

namespace Novino;

public static class NovinSwaggerApplicationExtensions
{
  public static INovinoApplication UseSwagger(this INovinoApplication app)
  {
    app.ApplicationBuilder.UseSwagger();
    app.ApplicationBuilder.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
      options.SwaggerEndpoint($"/swagger/{app.ServiceOptions.Version}/swagger.json", app.ServiceOptions.Name);
    });
    return app;
  }
}
