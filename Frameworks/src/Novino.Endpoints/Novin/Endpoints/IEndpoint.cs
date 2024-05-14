﻿using FluentValidation.Results;

namespace Novin.Endpoints;

public interface IEndpoint
{
  /// <summary>
  /// the http context of the current request
  /// </summary>
  HttpContext HttpContext { get; } //this is for allowing consumers to write extension methods

  /// <summary>
  /// validation failures collection for the endpoint
  /// </summary>
  List<ValidationFailure> ValidationFailures { get; } //also for extensibility

  /// <summary>
  /// gets the endpoint definition which contains all the configuration info for the endpoint
  /// </summary>
  EndpointDefinition Definition { get; } //also for extensibility

  void Initialize();

  Task ExecuteAsync(CancellationToken cancellationToken = default);
  
}
