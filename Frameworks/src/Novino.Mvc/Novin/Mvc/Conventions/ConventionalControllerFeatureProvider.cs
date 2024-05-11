using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Novin.Mvc.Conventions;

public class ConventionalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        var res = typeInfo.IsAssignableTo(typeof(IEndpoint)) && typeInfo is { IsAbstract: false, IsClass: true };
        return res;
    }
}