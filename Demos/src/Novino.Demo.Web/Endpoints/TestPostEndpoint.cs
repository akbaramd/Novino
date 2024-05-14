using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Novin.Endpoints;

namespace Novino.Demo.Web.Endpoints;

public class TestPostEndpoint(IServiceProvider serviceProvider) : Endpoint<TestPostEndpointRequest,TestPostEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    Put("/TestPost/{id}");
    Tag("Test","Testsda asdasd");
  }

  protected override async Task HandleAsync(TestPostEndpointRequest? request,CancellationToken cancellationToken = default)
  {
    await SendResponseAsync(new TestPostEndpointResponse { Status = request?.Id .ToString()??"" }, cancellationToken);
  }
}

public class TestPostEndpointResponse
{
  public string Status { get; set; } = default!;
}


public class TestPostEndpointRequest
{
  [FromRoute(Name = "id")]
  public Guid Id { get; set; } = default!;
  
  public int  Take { get; set; } = 10;
  public int  Skip { get; set; } = 0;
}


public class TestPostEndpointRequestValidator : AbstractValidator<TestPostEndpointRequest>
{
  public TestPostEndpointRequestValidator()
  {
    RuleFor(x => x.Id).NotNull().NotEmpty();
  }
}
