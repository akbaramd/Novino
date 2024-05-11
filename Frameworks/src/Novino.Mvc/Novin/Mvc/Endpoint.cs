namespace Novin.Mvc;

public abstract class Endpoint<TResponse> : IEndpoint
{
    public abstract Task<TResponse> HandleAsync();
    public HttpContext HttpContext { get; } = default!;
}