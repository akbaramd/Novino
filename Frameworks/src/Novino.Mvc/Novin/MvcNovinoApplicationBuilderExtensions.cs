using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Novin.Mvc.Conventions;
using Novino.Abstractions;

namespace Novin;

public static class MvcNovinoApplicationBuilderExtensions
{
    public static INovinoApplicationBuilder AddMvc(this INovinoApplicationBuilder builder)
    {
        var mvcCoreBuilder = builder.Services.AddMvcCore(options => { });

        builder.Services.AddMvc();

        //Add feature providers
        var partManager = (ApplicationPartManager)builder.Services
            .First(x => x.ServiceType == typeof(ApplicationPartManager)).ImplementationInstance!;

        partManager.FeatureProviders.Add(new ConventionalControllerFeatureProvider());

        builder.Services.AddOptions<MvcOptions>()
            .Configure<IServiceProvider>((mvcOptions, serviceProvider) =>
            {
                mvcOptions.Conventions.Add(new AbpServiceConventionWrapper(builder.Services));
            });
        return builder;
    }
    
    public static INovinoApplication UseMvc(this INovinoApplication app)
    {
        app.ApplicationBuilder.UseRouting();
        app.ApplicationBuilder.UseEndpoints(c =>
        {
            c.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            c.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });
        return app;
    }
}