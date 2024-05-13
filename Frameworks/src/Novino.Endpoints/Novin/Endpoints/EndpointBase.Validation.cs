using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public abstract partial class EndpointBase<TResponse>
{
  public List<ValidationFailure> ValidationFailures { get; } = [];

  public Task SendValidationErrorAsync(CancellationToken cancellationToken = default)
    => SendErrorResponseAsync(nameof(HttpStatusCode.BadRequest),
      string.Join(" | ", ValidationFailures.Select(x => x.ErrorMessage)), HttpStatusCode.Unauthorized,
      ValidationFailures,
      cancellationToken: cancellationToken);

  public void AddValidationError(params ValidationFailure[] failure)
  {
    ValidationFailures.AddRange(failure);
  }

  public void AddValidationError(List<ValidationFailure> failure)
    => AddValidationError(failure.ToArray());
}
