using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public abstract class Endpoint<TResponse>(IServiceProvider serviceProvider) : EndpointBase<TResponse>(serviceProvider)
{
 
  public override void Initialize()
  {
    Configure();
    
    Description(builder =>
    {
      builder.Produces<NovinErrorEndpointResponse<TResponse>>(500, "applciation/json");
      builder.Produces<TResponse>(200, "applciation/json");
    });

  }

  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      await HandleAsync(cancellationToken);
    }
    catch (Exception e)
    {
      await SendExceptionAsync(e, cancellationToken);
      throw;
    }
  }

  protected abstract Task HandleAsync(CancellationToken cancellationToken = default);
}

public abstract class Endpoint<TRequest, TResponse>(
  IServiceProvider serviceProvider)
  : EndpointBase<TResponse>(serviceProvider), IEndpoint where TRequest : notnull
{
  private readonly IValidator<TRequest>? _validator = serviceProvider.GetService<IValidator<TRequest>>();

 
  public override void Initialize()
  {
    Configure();

    Description(builder =>
    {
      builder.Produces<TResponse>(200, "applciation/json");
      builder.Produces<NovinErrorEndpointResponse<TResponse>>(500, "applciation/json");
      builder.Produces<NovinErrorEndpointResponse<TResponse>>(400, "applciation/json");
      builder.Accepts<TRequest>("application/json");
    });
  }

  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var request = GetRequest(cancellationToken);
      await ValidateRequestAsync(request, cancellationToken);
      await HandleAsync(request, cancellationToken);
    }
    catch (Exception e)
    {
      await SendExceptionAsync(e, cancellationToken);
    }
  }

  private TRequest? GetRequest(CancellationToken cancellationToken)
  {
    if (Definition.Method == HttpMethods.Get || Definition.Method == HttpMethods.Delete)
      return GetValueFromQuery();

    return HttpContext.Request.ReadFromJsonAsync<TRequest>(cancellationToken).Result;
  }

  private TRequest GetValueFromQuery()
  {
    var request = Activator.CreateInstance<TRequest>();

    foreach (var property in request!.GetType().GetProperties())
    {
      var value = GetValueFromAttribute(property);
      SetProperty(property, request, value);
    }

    return request;
  }

  private string? GetValueFromAttribute(PropertyInfo property)
  {
    var fromHeaderAttribute = property.GetCustomAttribute<FromHeaderAttribute>();
    var fromRouteAttribute = property.GetCustomAttribute<FromRouteAttribute>();
    var fromQueryAttribute = property.GetCustomAttribute<FromQueryAttribute>();

    if (fromHeaderAttribute != null)
    {
      return HttpContext.Request.Headers[fromHeaderAttribute.Name ?? property.Name].ToString();
    }

    if (fromRouteAttribute != null)
    {
      var value = HttpContext.Request.RouteValues[fromRouteAttribute.Name ?? property.Name];
      return value?.ToString();
    }

    if (fromQueryAttribute != null)
    {
      var value = HttpContext.Request.Query[fromQueryAttribute.Name ?? property.Name];
      return value.ToString();
    }

    return null;
  }

  private async Task ValidateRequestAsync(TRequest? request, CancellationToken cancellationToken)
  {
    if (_validator != null && request != null)
    {
      var validateResult = await _validator.ValidateAsync(request, cancellationToken);
      if (!validateResult.IsValid)
      {
        AddValidationError(validateResult.Errors);
        await SendValidationErrorAsync(cancellationToken);
        throw new ArgumentException("Validation failed");
      }
    }
  }

  private void SetProperty(PropertyInfo property, TRequest request, string? value)
  {
    if (value == null) return;

    foreach (var conversionFunction in EndpointOptions.Value.Conversions
               .Where(conversionFunction => property.PropertyType == conversionFunction.targetType))
    {
      property.SetValue(request, conversionFunction.mapTo.DynamicInvoke(value));
      return;
    }

    throw new ArgumentException($"Failed to set property '{property.Name}' with value '{value}'");
  }

  protected abstract Task HandleAsync(TRequest? request, CancellationToken cancellationToken);
}
