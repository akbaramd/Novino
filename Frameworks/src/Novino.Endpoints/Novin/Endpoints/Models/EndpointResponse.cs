using System.Net;
using FluentValidation;
using FluentValidation.Results;

namespace Novin.Endpoints.Models;



public class EndpointErrorResponse<TResponse> 
{
  public HttpStatusCode HttpStatus { get; set; } = default!;
  public string ErrorMessage { get; set; } = default!;
  public string ErrorCode { get; set; }= default!;
  public List<ValidationFailure> Validations { get; set; } =[];
}
