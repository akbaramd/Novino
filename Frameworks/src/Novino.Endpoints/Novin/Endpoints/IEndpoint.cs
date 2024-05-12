using FluentValidation.Results;

namespace Novin.Endpoints;

public interface IEndpoint
{
  internal string HttpMethod { get; set; }
  public string Pattern { get; set; }
  /// <summary>
  /// the http context of the current request
  /// </summary>
  HttpContext? HttpContext { get; } //this is for allowing consumers to write extension methods
  public EndpointConfiguration EndpointConfiguration { get; set; }
  
  public Task ExecuteAsync(CancellationToken cancellationToken = default);

  public void Configure();
}

public interface IEndpoint<in TRequest>  : IEndpoint
{
 
  public List<ValidationFailure> ValidationFailures { get; }

}


