using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Novin.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProducesResponseTypeMetadata = Novin.Endpoints.Summary.ProducesResponseTypeMetadata;

namespace Novino.Swagger.Endpoints;

public class NovinEndpointOperations : IOperationFilter
{
  static readonly string[] _illegalHeaderNames = ["Accept", "Content-Type", "Authorization"];
  static readonly Dictionary<string, string> _defaultDescriptions = new()
  {
    { "200", "Success" },
    { "201", "Created" },
    { "202", "Accepted" },
    { "204", "No Content" },
    { "400", "Bad Request" },
    { "401", "Unauthorized" },
    { "403", "Forbidden" },
    { "404", "Not Found" },
    { "405", "Method Not Allowed" },
    { "406", "Not Acceptable" },
    { "429", "Too Many Requests" },
    { "500", "Server Error" }
  };
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    var metaData = context.ApiDescription.ActionDescriptor.EndpointMetadata;
    var epDef = metaData.OfType<EndpointDefinition>()
      .SingleOrDefault(); //use shortcut `ctx.GetEndpointDefinition()` for your own processors

    if (epDef is null)
      return;

    var apiDescription = context.ApiDescription;
    var reqContent = operation.RequestBody?.Content;
    var serializer = JsonSerializer.Create();

   
        //fix response content-types not displaying correctly
        //also set user provided response examples and response headers
        if (operation.Responses.Count > 0)
        {
            var metas = metaData
                        .OfType<ProducesResponseTypeMetadata>()
                        .GroupBy(
                            m => m.StatusCode,
                            (k, g) =>
                            {
                                var meta = g.Last();
                                object? example = null;
                                _ = epDef.EndpointSummary?.ResponseExamples.TryGetValue(k, out example);
                                example = meta?.Example ?? example;
                                example = example is not null ? JToken.FromObject(example, serializer) : null;

                                if ( example is JToken { Type: JTokenType.Array } token)
                                    example = token.ToString();

                                return new
                                {
                                    key = k.ToString(),
                                    cTypes = meta?.ContentTypes,
                                    example,
                                    usrHeaders = epDef.EndpointSummary?.ResponseHeaders.Where(h => h.StatusCode == k).ToArray(),
                                    tDto = meta?.Type
                                };
                            })
                        .ToDictionary(x => x.key);

            if (metas.Count > 0)
            {
                foreach (var rsp in operation.Responses)
                {
                    var cTypes = metas[rsp.Key].cTypes;
                    var mediaType = rsp.Value.Content.FirstOrDefault().Value;

                    if (metas.TryGetValue(rsp.Key, out var x))
                    {
                        if (x.example is not null)
                            mediaType.Example = (IOpenApiAny)x.example;

                        foreach (var p in x.tDto!
                                           .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                           )
                        {
                           
                        }

                        
                    }

                    rsp.Value.Content.Clear();
                    if (cTypes != null)
                    {
                      foreach (var ct in cTypes)
                        rsp.Value.Content.Add(new(ct, mediaType));
                    }
                }
            }
        }

        //set endpoint summary & description
        operation.Summary = epDef.EndpointSummary?.Summary ;
        operation.Description = epDef.EndpointSummary?.Description;
        
        operation.Responses
          .Where(r => string.IsNullOrWhiteSpace(r.Value.Description))
          .ToList()
          .ForEach(
              oaResp =>
              {
                  //first set the default descriptions
                  if (_defaultDescriptions.TryGetValue(oaResp.Key, out var description))
                      oaResp.Value.Description = description;

                  var statusCode = Convert.ToInt32(oaResp.Key);

                  //then override with user supplied values from EndpointSummary.Responses
                  if (epDef.EndpointSummary?.Responses.ContainsKey(statusCode) is true)
                      oaResp.Value.Description = epDef.EndpointSummary.Responses[statusCode];

                  //set response dto property descriptions
                  if (epDef.EndpointSummary?.ResponseParams.ContainsKey(statusCode) is true )
                  {
                      var propDescriptions = epDef.EndpointSummary.ResponseParams[statusCode];
                      var respDtoProps = apiDescription
                                         .SupportedResponseTypes
                                         .SingleOrDefault(x => x.StatusCode == statusCode)?
                                         .Type?
                                         .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                         .Select(
                                             p => new
                                             {
                                                 key = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name,
                                                 val = p.Name
                                             })
                                         .Where(x => x.key is not null)
                                         .ToDictionary(x => x.key!, x => x.val);

                      // foreach (var prop in oaResp.Value.GetAllProperties())
                      // {
                          // string? propName = null;
                          // respDtoProps?.TryGetValue(prop.Key, out propName);
                          // propName ??= prop.Key;

                          // if (propDescriptions.TryGetValue(propName, out var responseDescription))
                              // prop.Value.Description = responseDescription;
                      // }
                  }
              });
  }
}
