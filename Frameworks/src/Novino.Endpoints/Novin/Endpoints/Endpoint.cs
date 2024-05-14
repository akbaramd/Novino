using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Novin.Endpoints;

public abstract class Endpoint<TResponse>(IServiceProvider serviceProvider)
  : EndpointBase<TResponse>(serviceProvider)
{
  public override void Initialize()
  {
    Configure();
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

  public abstract Task HandleAsync(CancellationToken cancellationToken = default);
}

public abstract class Endpoint<TRequest, TResponse>(IServiceProvider serviceProvider)
  : EndpointBase<TResponse>(serviceProvider), IEndpoint
{
  private IValidator<TRequest>? Validator => ServiceProvider.GetService<IValidator<TRequest>>();

  public override void Initialize()
  {
    Configure();
    
    Definition.Parameters = new List<EndpointDocumentParameter>()
    {
      new()
      {
        In = Definition.Method == HttpMethods.Get ? "query" : "body",
        Name = typeof(TRequest).Name,
        Type = typeof(TRequest),
        Example = typeof(TRequest).GetDefaultInstance()
      }
    };
  
  }
  
  public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      TRequest? request;
      if (Definition.Method == HttpMethods.Get || Definition.Method == HttpMethods.Delete)
      {
        request = GetValueFromQuery();
      }
      else
      {
        request = await HttpContext.Request.ReadFromJsonAsync<TRequest>(cancellationToken);
      }
      
       
      if (Validator is not null)
      {
        if (request != null)
        {
          if (Validator != null)
          {
            var validateResult = await Validator.ValidateAsync(request, cancellationToken);
            if (!validateResult.IsValid)
            {
               AddValidationError(validateResult.Errors);
               await SendValidationErrorAsync(cancellationToken);
              return;
            }

          }
        }
      }


      await HandleAsync(request, cancellationToken);
    }
    catch (Exception e)
    {
      await SendExceptionAsync(e, cancellationToken);
    }
  }

  private TRequest GetValueFromQuery()
  {
    var request = Activator.CreateInstance<TRequest>();

    foreach (var property in request?.GetType().GetProperties()!)
    {
      var fromRouteAttribute = property.GetCustomAttribute<FromRouteAttribute>();
      var fromHeaderAttribute = property.GetCustomAttribute<FromHeaderAttribute>();
      var fromQueryAttribute = property.GetCustomAttribute<FromQueryAttribute>();

      if (fromHeaderAttribute != null)
      {
        var name = fromHeaderAttribute.Name ?? property.Name;
        HttpContext.Request.Headers.TryGetValue(name, out var value);
        SetProperty(property, request, value);
      }
      else if (fromRouteAttribute != null)
      {
        var name = fromRouteAttribute.Name ?? property.Name;
        HttpContext.Request.RouteValues.TryGetValue(name, out var value);
        SetProperty(property, request, value?.ToString());

      }
      else 
      {
        var name = fromQueryAttribute?.Name ?? property.Name;
        HttpContext.Request.Query.TryGetValue(name, out var value);
        SetProperty(property, request, value);
      }
    }
    
    return request;
  }

  private static void SetProperty(PropertyInfo property, [DisallowNull] TRequest request, string? value)
  {
    if (value == null)
    {
      // No value to set, so exit early
      return;
    }

    // Check if property is nullable
    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);

    // Convert value to the appropriate type
    object? convertedValue = null;
    if (property.PropertyType == typeof(string))
    {
      // For string properties, no conversion needed
      convertedValue = value;
    }
    else if (property.PropertyType.IsEnum)
    {
      // For enum properties, parse the enum value
      convertedValue = Enum.Parse(property.PropertyType, value);
    }
    else if (underlyingType != null)
    {
      // For nullable properties, handle conversion to the underlying type
      if (string.IsNullOrEmpty(value))
      {
        convertedValue = null;
      }
      else
      {
        convertedValue = Convert.ChangeType(value, underlyingType);
      }
    }
    else
    {
      // For non-nullable properties, handle conversion directly
      convertedValue = Convert.ChangeType(value, property.PropertyType);
    }

    // Set the property value
    property.SetValue(request, convertedValue);
    
  }

  protected abstract Task HandleAsync(TRequest? request, CancellationToken cancellationToken = default);
}
