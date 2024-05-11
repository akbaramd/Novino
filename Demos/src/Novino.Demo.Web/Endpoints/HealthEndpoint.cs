using System.ComponentModel;
using Novin.Mvc;

namespace Novino.Demo.Web.Endpoints;

public class HealthEndpoint : Endpoint<HealthEndpointResponse>
{
    
    public override async Task<HealthEndpointResponse> HandleAsync()
    {
        Console.WriteLine(HttpContext.Request.Path);
        return new HealthEndpointResponse()
        {
            Status = "Success"
        };
    }
}


public class HealthEndpointResponse
{
    public string Status { get; set; }
}