using FluentValidation;

namespace Novin.Endpoints;

public abstract partial class Endpoint<TResponse>(IServiceProvider serviceProvider)
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
        Name = typeof(TRequest)?.Name,
        Type = typeof(TRequest),
        Example = typeof(TRequest)?.GetDefaultInstance()
      }
    };
  
  }
  
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

  public abstract Task HandleAsync(TRequest? request, CancellationToken cancellationToken = default);
}
