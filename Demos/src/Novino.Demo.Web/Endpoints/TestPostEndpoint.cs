using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Novin.Endpoints;

namespace Novino.Demo.Web.Endpoints;

public class TestPostEndpoint(IServiceProvider serviceProvider) : Endpoint<TestPostEndpointRequest,TestPostEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    Get("/TestPost");
    Tag("Test","Testsda asdasd");
    
    Response(c =>
    {
      c.Add(new EndpointDocumentResponse()
      {
        StatusCode =  HttpStatusCode.OK,
        Example = new TestPostEndpointResponse()
        {
          Status = "Salam"
        }
      });
    });
  }

  public override async Task HandleAsync(TestPostEndpointRequest? request,CancellationToken cancellationToken = default)
  {
    await SendResponseAsync(new TestPostEndpointResponse { Status = request?.Status??"" }, cancellationToken);
  }
}

public class TestPostEndpointResponse
{
  public string Status { get; set; } = default!;
}


public class TestPostEndpointRequest
{
  public string Status { get; set; } = default!;
}


public class TestPostEndpointRequestValidator : AbstractValidator<TestPostEndpointRequest>
{
  public TestPostEndpointRequestValidator()
  {
    RuleFor(x => x.Status).NotNull().NotEmpty();
  }
}
