namespace Novin.Mvc;

public interface IEndpoint 
{
    /// <summary>
    /// the http context of the current request
    /// </summary>
    HttpContext HttpContext { get; } //this is for allowing consumers to write extension methods
}