using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Novin.Endpoints.Models;

namespace Novin.Endpoints;

public abstract partial class EndpointBase<TResponse>
{
  public EndpointDefinition Definition { get; private set; } = new();


  public virtual EndpointBase<TResponse> Verbs(string method)
  {
    Definition.Method = method;
    return this;
  }

  public virtual EndpointBase<TResponse> Path(string path)
  {
    if (!path.StartsWith("/"))
    {
      path = "/" + path;
    }

    Definition.Path = path.Replace("//","/");
    return this;
  }

  public virtual EndpointBase<TResponse> Verbs(HttpMethod method)
  {
    Definition.Method = method.Method;
    return this;
  }

  public virtual EndpointBase<TResponse> Get([StringSyntax("Route")] string path)
    => Verbs(HttpMethod.Get)
      .Path(path);

  public virtual EndpointBase<TResponse> Post([StringSyntax("Route")] string path)
    => Verbs(HttpMethod.Post)
      .Path(path);

  public virtual EndpointBase<TResponse> Put([StringSyntax("Route")] string path)
    => Verbs(HttpMethod.Put)
      .Path(path);

  public virtual EndpointBase<TResponse> Delete([StringSyntax("Route")] string path)
    => Verbs(HttpMethod.Delete)
      .Path(path);

  public virtual EndpointBase<TResponse> Patch([StringSyntax("Route")] string path)
    => Verbs(HttpMethod.Patch)
      .Path(path);


  protected void Tag(string name, string description)
  {
    Definition.DocumentTag = new EndpointDocumentTag() { Name = name, Description = description };
  }

}
