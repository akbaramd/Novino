using System.Diagnostics.CodeAnalysis;
using Quartz;

namespace Novino.Scheduler.Quartz;

public class NovinQuartzConfigure
{
    public NovinQuartzConfigure(IServiceCollectionQuartzConfigurator quartzConfigurator)
    {
        QuartzConfigurator = quartzConfigurator;
    }

    private IServiceCollectionQuartzConfigurator QuartzConfigurator { get; set; }

    public void AddCronJob<TJob>( [StringSyntax("cron")] string pattern) where TJob : IJob
    {
        var jobKey = new JobKey(typeof(TJob).Name.ToKebabCase());
        QuartzConfigurator.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
        QuartzConfigurator.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobKey.Name)
            .WithCronSchedule(pattern)
        );
    }
        
}