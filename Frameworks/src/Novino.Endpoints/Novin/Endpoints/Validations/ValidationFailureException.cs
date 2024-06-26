﻿using FluentValidation.Results;

namespace Novin.Endpoints.Validations;

public sealed class ValidationFailureException : Exception
{
  /// <summary>
  /// the collection of failures that have occured.
  /// </summary>
  public IEnumerable<ValidationFailure>? Failures { get; init; }

  /// <summary>
  /// the status code to be used when building the error response.
  /// </summary>
  public int? StatusCode { get; internal init; }

  public ValidationFailureException() { }

  public ValidationFailureException(string? message) : base(message) { }

  public ValidationFailureException(string? message, Exception? innerException) : base(message, innerException) { }

  public ValidationFailureException(IEnumerable<ValidationFailure> failures, string message)
    : base($"{message} - {failures.FirstOrDefault()?.ErrorMessage}")
  {
    Failures = failures;
  }
}