using System.Diagnostics.CodeAnalysis;
using Quartz;

namespace Novino.Scheduler.Quartz;

public class NovinQuartzConfigure
{
  public NovinQuartzConfigure(IServiceCollectionQuartzConfigurator quartzConfigurator)
  {
    QuartzConfigurator = quartzConfigurator;
  }

  private IServiceCollectionQuartzConfigurator QuartzConfigurator { get; }

  public void AddCronJob<TJob>([StringSyntax("cron")] string pattern, bool fireOnStart = true) where TJob : IJob
  {
    var jobKey = new JobKey(typeof(TJob).Name.ToKebabCase() + "-cron");
    QuartzConfigurator.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
    QuartzConfigurator.AddTrigger(opts =>
      {
        opts
          .ForJob(jobKey)
          .WithIdentity(jobKey.Name)
          .WithCronSchedule(pattern);

        opts.StartNow();
      }
    );
  }

  public void AddIntervalJob<TJob>(TimeSpan interval, bool repeat = true, bool fireOnStart = true) where TJob : IJob
  {
    var jobKey = new JobKey(typeof(TJob).Name.ToKebabCase() + "-interval");
    QuartzConfigurator.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
    QuartzConfigurator.AddTrigger(opts =>
      {
        opts
          .ForJob(jobKey)
          .WithIdentity(jobKey.Name);

        if (fireOnStart)
        {
          opts.StartNow();
        }

        opts.WithSimpleSchedule(v =>
        {
          if (fireOnStart)
          {
          }

          v.WithInterval(interval);
          if (repeat)
          {
            v.RepeatForever();
          }

          v.Build();
        });
      }
    );
  }
}
