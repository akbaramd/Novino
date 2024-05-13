using System.Net;

namespace Novin.Endpoints;

public class NovinoEndpointDefinitions : List<EndpointDefinition>
{
}

public class EndpointDefinition
{
  public string Method { get; set; } = string.Empty;
  public  string Path { get; set; }= string.Empty;
  public  string Description { get; set; }= string.Empty;
  public EndpointDocumentTag DocumentTag { get; set; } = EndpointDocumentTag.Default;
  public IEnumerable<EndpointDocumentParameter> Parameters { get; set; } = new List<EndpointDocumentParameter>();
  public List<EndpointDocumentResponse> Responses { get; set; } = new List<EndpointDocumentResponse>();

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