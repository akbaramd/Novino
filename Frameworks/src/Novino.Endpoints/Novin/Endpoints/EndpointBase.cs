using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public  abstract partial class EndpointBase<TResponse>(IServiceProvider serviceProvider) : IEndpoint
{
  public IServiceProvider ServiceProvider { get; } = serviceProvider;
  protected IOptions<NovinoEndpointsOptions> EndpointOptions => ServiceProvider.GetRequiredService<IOptions<NovinoEndpointsOptions>>();
  public abstract void Initialize();

  public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);


  public abstract void Configure();

}
