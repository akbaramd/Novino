using Novino.Abstractions;
using Novino.Scheduler;
using Novino.Scheduler.Quartz;
using Quartz;
using Quartz.AspNetCore;

namespace Novino;

public static class NovinSchedulerExtensions
{
    public static INovinoBuilder AddQuartz(this INovinoBuilder builder,
        Action<NovinQuartzConfigure> configurator)
    {
        
        builder.Services.AddQuartz(c =>
        {  
            configurator.Invoke(new NovinQuartzConfigure(c));
        });
        builder.Services.AddQuartzServer();
        builder.Services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
        return builder;
    }
}