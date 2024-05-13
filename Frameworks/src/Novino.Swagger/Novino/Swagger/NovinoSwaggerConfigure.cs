using Swashbuckle.AspNetCore.SwaggerGen;

namespace Novino.Swagger;

public class NovinoSwaggerConfigure : INovinoSwaggerConfigure
{
  public NovinoSwaggerConfigure(SwaggerGenOptions options)
  {
    Options = options;
  }

  public SwaggerGenOptions Options { get; }
}
