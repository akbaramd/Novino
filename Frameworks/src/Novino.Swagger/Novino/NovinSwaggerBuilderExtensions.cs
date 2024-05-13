using Microsoft.OpenApi.Models;
using Novino.Abstractions;
using Novino.Swagger;

namespace Novino;

public static class NovinSwaggerBuilderExtensions
{
    public static INovinoBuilder AddSwagger(this INovinoBuilder builder , Action<INovinoSwaggerConfigure>? configure = null)
    {
      builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
          configure?.Invoke(new NovinoSwaggerConfigure(options));
          // options.DocumentFilter<NovinDocumentFilter>();
          options.SwaggerDoc(builder.ServiceOptions.Version, new OpenApiInfo
          {
            Version = builder.ServiceOptions.Version,
            Title = builder.ServiceOptions.Name,
            Description = builder.ServiceOptions.Description,
          });
          
        });
        return builder;
    }
}