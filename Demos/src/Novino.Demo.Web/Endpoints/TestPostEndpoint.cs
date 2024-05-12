using FluentValidation;
using Novin.Endpoints;

namespace Novino.Demo.Web.Endpoints;

public class TestPostEndpoint(IServiceProvider serviceProvider) : Endpoint<TestPostEndpointRequest,TestPostEndpointResponse>(serviceProvider)
{
  public override void Configure()
  {
    MapPost("/TestPost");
    
  }

  public override async Task HandleAsync(TestPostEndpointRequest? request,CancellationToken cancellationToken = default)
  {
    await SendOkResponseAsync(new TestPostEndpointResponse { Status = request?.Status??"" }, cancellationToken);
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
