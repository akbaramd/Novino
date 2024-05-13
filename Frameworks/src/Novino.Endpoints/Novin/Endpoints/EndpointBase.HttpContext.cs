using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public abstract partial class EndpointBase<TResponse>
{
  public HttpContext HttpContext => ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!;


  public T? RouteValue<T>(string name)
  {
    try
    {
      return (T?)HttpContext.Request.RouteValues[name];
    }
    catch (Exception)
    {
      return default;
    }
  }


  public async Task SendResponseAsync(TResponse response, HttpStatusCode statusCode = HttpStatusCode.OK,
    CancellationToken cancellationToken = default)
  {
    HttpContext.Response.StatusCode = (int)statusCode;
    await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
  }

  public async Task SendErrorResponseAsync(string errorCode, string message,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError, List<ValidationFailure>? validationFailures = null,
    CancellationToken cancellationToken = default)
  {
    HttpContext.Response.StatusCode = (int)statusCode;
    await HttpContext.Response.WriteAsJsonAsync(
      new NovinErrorEndpointResponse<TResponse>()
      {
        Validations = validationFailures, ErrorCode = errorCode, ErrorMessage = message, HttpStatus = statusCode
      }, cancellationToken);
  }

  public Task SendResponseAsync(TResponse response, CancellationToken cancellationToken = default)
    => SendResponseAsync(response, HttpStatusCode.OK, cancellationToken);

  public Task SendCreatedResponseAsync(TResponse response, CancellationToken cancellationToken = default)
    => SendResponseAsync(response, HttpStatusCode.Created, cancellationToken);

  public Task SendAcceptedResponseAsync(TResponse response, CancellationToken cancellationToken = default)
    => SendResponseAsync(response, HttpStatusCode.Accepted, cancellationToken);

  public Task SendExceptionAsync(Exception exception, CancellationToken cancellationToken = default)
    => SendErrorResponseAsync(nameof(Exception), exception.Message, cancellationToken: cancellationToken);

  public Task SendUnauthorizedAsync(string message, CancellationToken cancellationToken = default)
    => SendErrorResponseAsync(nameof(HttpStatusCode.Unauthorized), message, HttpStatusCode.Unauthorized,
      cancellationToken: cancellationToken);
  
  public Task SendBadRequestAsync(string message, CancellationToken cancellationToken = default)
    => SendErrorResponseAsync(nameof(HttpStatusCode.BadRequest), message, HttpStatusCode.Unauthorized,
      cancellationToken: cancellationToken);
  
  public Task SendNotFoundAsync(string message, CancellationToken cancellationToken = default)
    => SendErrorResponseAsync(nameof(HttpStatusCode.NotFound), message, HttpStatusCode.Unauthorized,
      cancellationToken: cancellationToken);
}
