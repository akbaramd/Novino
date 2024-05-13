using Swashbuckle.AspNetCore.SwaggerGen;

namespace Novino.Swagger;

public interface INovinoSwaggerConfigure
{
  public SwaggerGenOptions Options { get;  }
}
