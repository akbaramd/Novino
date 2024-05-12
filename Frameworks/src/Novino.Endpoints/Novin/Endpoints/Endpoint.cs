using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public abstract class EndpointBase<TResponse>(IServiceProvider serviceProvider) : IEndpoint
{
  public IServiceProvider ServiceProvider { get; } = serviceProvider;

  public string HttpMethod { get; set; } = HttpMethods.Get;
  public string Pattern { get; set; } = string.Empty;
  public HttpContext HttpContext => ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!;
  public EndpointConfiguration EndpointConfiguration { get; set; } = new();

  public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

  public virtual void Configure()
  {
    if (string.IsNullOrWhiteSpace(Pattern))
    {
      Pattern = GetType().Name;
    }
  }


  public async Task SendResponseAsync(TResponse response, HttpStatusCode statusCode = HttpStatusCode.OK,
    CancellationToken cancellationToken = default)
  {
    HttpContext.Response.StatusCode = (int) statusCode;
    await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
  }

  public Task SendOkResponseAsync(TResponse response, CancellationToken cancellationToken = default)
  {
    return SendResponseAsync(response, HttpStatusCode.OK, cancellationToken);
  }

  public Task SendCreatedResponseAsync(TResponse response, CancellationToken cancellationToken = default)
  {
    return SendResponseAsync(response, HttpStatusCode.Created, cancellationToken);
  }

  public Task SendAcceptedResponseAsync(TResponse response, CancellationToken cancellationToken = default)
  {
    return SendResponseAsync(response, HttpStatusCode.Accepted, cancellationToken);
  }

   internal async Task SendValidationResponseAsync(List<ValidationFailure> failures,
    CancellationToken cancellationToken = default)
   {
      HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    await HttpContext.Response.WriteAsJsonAsync(
      new EndpointErrorResponse<TResponse>
      {
        HttpStatus = HttpStatusCode.BadRequest,
        ErrorCode = nameof(HttpStatusCode.BadRequest),
        ErrorMessage = nameof(HttpStatusCode.BadRequest),
        Validations = failures
      }, cancellationToken);
  }

  public async Task SendErrorResponseAsync(string errorCode, string exception,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError, CancellationToken cancellationToken = default)
  {
    HttpContext.Response.StatusCode = (int) statusCode;
    await HttpContext.Response.WriteAsJsonAsync(
      new EndpointErrorResponse<TResponse>
      {
        HttpStatus = HttpStatusCode.InternalServerError, ErrorCode = errorCode, ErrorMessage = exception
      }, cancellationToken);
  }


  public Task SendErrorResponseAsync(string exception, CancellationToken cancellationToken = default)
  {
    return SendErrorResponseAsync(nameof(HttpStatusCode.InternalServerError), exception,
      HttpStatusCode.InternalServerError, cancellationToken);
  }

  public Task SendErrorResponseAsync(string exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
    CancellationToken cancellationToken = default)
  {
    return SendErrorResponseAsync(statusCode.ToString(), exception, statusCode, cancellationToken);
  }

  public Task SendBadRequestAsync(string exception, CancellationToken cancellationToken = default)
  {
    return SendErrorResponseAsync(nameof(HttpStatusCode.BadRequest), exception, HttpStatusCode.BadRequest,
      cancellationToken);
  }

  public Task SendNotFoundAsync(string exception, CancellationToken cancellationToken = default)
  {
    return SendErrorResponseAsync(nameof(HttpStatusCode.NotFound), exception, HttpStatusCode.NotFound,
      cancellationToken);
  }

  public Task SendUnAuthorizedRequestAsync(string exception, CancellationToken cancellationToken = default)
  {
    return SendErrorResponseAsync("Unauthorized", exception, HttpStatusCode.Unauthorized, cancellationToken);
  }

  public void MapGet([StringSyntax("Route")] string pattern)
  {
    HttpMethod = HttpMethods.Get;
    Pattern = pattern;
  }

  public void MapPost([StringSyntax("Route")] string pattern)
  {
    HttpMethod = HttpMethods.Post;
    Pattern = pattern;
  }

  public void MapPut([StringSyntax("Route")] string pattern)
  {
    HttpMethod = HttpMethods.Put;
    Pattern = pattern;
  }

  public void MapDelete([StringSyntax("Route")] string pattern)
  {
    HttpMethod = HttpMethods.Delete;
    Pattern = pattern;
  }

  public void MapPatch([StringSyntax("Route")] string pattern)
  {
    HttpMethod = HttpMethods.Patch;
    Pattern = pattern;
  }


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
}

public abstract class Endpoint<TResponse>(IServiceProvider serviceProvider)
  : EndpointBase<TResponse>(serviceProvider), IEndpoint
{
  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      await HandleAsync(cancellationToken);
    }
    catch (Exception e)
    {
      await SendErrorResponseAsync(e.Message, cancellationToken);
      throw;
    }
    
  
  }

  public abstract Task HandleAsync(CancellationToken cancellationToken = default);
}

public abstract class Endpoint<TRequest, TResponse>(IServiceProvider serviceProvider)
  : EndpointBase<TResponse>(serviceProvider), IEndpoint
{
  private IValidator<TRequest>? Validator => ServiceProvider.GetService<IValidator<TRequest>>();

  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var request = await HttpContext.Request.ReadFromJsonAsync<TRequest>(cancellationToken);
      if (Validator is not null)
      {
        if (request != null)
        {
          if (Validator != null)
          {
            var validateResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validateResult.IsValid)
            {
              await SendValidationResponseAsync(validateResult.Errors, cancellationToken);
              return;
            }

          }
        }
      }


      await HandleAsync(request, cancellationToken);
    }
    catch (Exception e)
    {
      await SendErrorResponseAsync(e.Message, cancellationToken);
    }
  }

  public abstract Task HandleAsync(TRequest? request, CancellationToken cancellationToken = default);
}
