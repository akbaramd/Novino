using Microsoft.OpenApi.Models;
using Novino.Abstractions;
using Novino.Swagger;
using Novino.Swagger.Endpoints;

namespace Novino;

public static class NovinSwaggerConfigureExtensions
{
    public static INovinoSwaggerConfigure AddEndpointsSwagger(this INovinoSwaggerConfigure builder)
    {
      builder.Options.DocumentFilter<NovinEndpointDocumentFilter>();
        return builder;
    }
}