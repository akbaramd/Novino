using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Novin.Mvc.Conventions;

public class AbpServiceConventionWrapper : IApplicationModelConvention
{
    private readonly IServiceCollection _services;

    public AbpServiceConventionWrapper(IServiceCollection services)
    {
        _services = services;
    }

    public void Apply(ApplicationModel application)
    {
    }
}