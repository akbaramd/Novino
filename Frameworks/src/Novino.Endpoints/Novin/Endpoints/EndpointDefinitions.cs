using System.Net;
using Microsoft.AspNetCore.Http.Metadata;
using Novin.Endpoints.Summary;
using ProducesResponseTypeMetadata = Novin.Endpoints.Summary.ProducesResponseTypeMetadata;

namespace Novin.Endpoints;

public class NovinoEndpointDefinitions : List<EndpointDefinition>
{
}

public class EndpointDefinition
{
  internal List<IProducesResponseTypeMetadata> ProducesMetas { get; } = new();
  internal Action<RouteHandlerBuilder>? UserConfigAction { get; private set; }
  public EndpointSummary? EndpointSummary { get; private set; } = new();
 
  public string Method { get; set; } = string.Empty;
  public  string Path { get; set; }= string.Empty;
  
  public EndpointDocumentTag DocumentTag { get; set; } = EndpointDocumentTag.Default;
  public IEnumerable<EndpointDocumentParameter> Parameters { get;  } = new List<EndpointDocumentParameter>();
  public Dictionary<int, string> Responses { get; set; } = new();
  
  /// <summary>
  /// describe openapi metadata for this endpoint. optionally specify whether or not you want to clear the default Accepts/Produces metadata.
  /// <para>
  /// EXAMPLE: <c>b => b.Accepts&lt;Request&gt;("text/plain")</c>
  /// </para>
  /// </summary>
  /// <param name="builder">the route handler builder for this endpoint</param>
  /// <param name="clearDefaults">set to true if the defaults should be cleared</param>
  public void Description(Action<RouteHandlerBuilder> builder, bool clearDefaults = false)
  {
    UserConfigAction = builder + UserConfigAction;
  } 
  
  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an action that sets values of an endpoint summary object</param>
  public void Summary(Action<EndpointSummary> endpointSummary)
    => endpointSummary(EndpointSummary ??= new());

  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an action that sets values of an endpoint summary object</param>
  public void Summary<TRequest>(Action<EndpointSummary<TRequest>> endpointSummary) where TRequest : notnull
  {
    var summary = EndpointSummary as EndpointSummary<TRequest> ?? new EndpointSummary<TRequest>();
    endpointSummary(summary);
    EndpointSummary = summary;
  }

  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an endpoint summary instance</param>
  public void Summary(EndpointSummary endpointSummary)
    => EndpointSummary = endpointSummary;
  
    /// <summary>
    /// add a response description to the swagger document
    /// <para>
    /// NOTE: if you use the this method, the default 200 response is automatically removed, and you'd have to specify the 200 response yourself if it
    /// applies to your endpoint.
    /// </para>
    /// </summary>
    /// <typeparam name="TResponse">the type of the response dto</typeparam>
    /// <param name="statusCode">http status code</param>
    /// <param name="description">the description of the response</param>
    /// <param name="contentType">the media/content type of the response</param>
    /// <param name="example">and example response dto instance</param>
    public void Response<TResponse>(int statusCode = 200,
                                    string? description = null,
                                    string contentType = "application/json",
                                    TResponse? example = default)
    {
        ProducesMetas.Add(
            new ProducesResponseTypeMetadata()
            {
                ContentTypes = new[] { contentType },
                StatusCode = statusCode,
                Type = typeof(TResponse),
                Example = example
            });

        if (description is not null)
            Responses[statusCode] = description;
    }

    /// <summary>
    /// add a response description that doesn't have a response dto to the swagger document
    /// NOTE: if you use the this method, the default 200 response is automatically removed, and you'd have to specify the 200 response yourself if it
    /// applies to your endpoint.
    /// </summary>
    /// <param name="statusCode">http status code</param>
    /// <param name="description">the description of the response</param>
    /// <param name="contentType">the media/content type of the response</param>
    public void Response(int statusCode = 200, string? description = null, string? contentType = null)
    {
        ProducesMetas.Add(
            new ProducesResponseTypeMetadata
            {
                ContentTypes = contentType is null ? Enumerable.Empty<string>() : new[] { contentType },
                StatusCode = statusCode,
                Type = typeof(void)
            });

        if (description is not null)
            Responses[statusCode] = description;
    }

}
    
public class EndpointDocumentParameter
{
  public string In { get; set; }= string.Empty;
  public Type? Type { get; set; } = default!;
  public string? Name { get; set; }= string.Empty;
  public object? Example { get; set; }= string.Empty;
}
    
public class EndpointDocumentResponse
{
  public HttpStatusCode StatusCode { get; set; }
  public object? Example { get; set; }= string.Empty;
}


public class EndpointDocumentTag
{
  public static EndpointDocumentTag Default = new ()
  {
    Name = nameof(Default),
    Description = ""
  };
  
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; } = string.Empty;
}