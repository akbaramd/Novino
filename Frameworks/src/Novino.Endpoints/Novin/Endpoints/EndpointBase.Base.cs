using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Novin.Endpoints.Models;
using Novin.Endpoints.Summary;

namespace Novin.Endpoints;

public  abstract partial class EndpointBase<TResponse>
{


  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an action that sets values of an endpoint summary object</param>
  protected void Description(Action<RouteHandlerBuilder> desc)
    => Definition.Description(desc); 


  
  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an action that sets values of an endpoint summary object</param>
  protected void Summary(Action<EndpointSummary> endpointSummary)
    => Definition.Summary(endpointSummary);


  /// <summary>
  /// provide a summary/description for this endpoint to be used in swagger/ openapi
  /// </summary>
  /// <param name="endpointSummary">an endpoint summary instance</param>
  protected void Summary(EndpointSummary endpointSummary)
    => Definition.Summary(endpointSummary);
}
