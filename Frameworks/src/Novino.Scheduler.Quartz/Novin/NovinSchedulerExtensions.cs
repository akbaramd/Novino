using Novino.Abstractions;
using Quartz;

namespace Novin;

public static class NovinSchedulerExtensions
{
  public static INovinoBuilder AddScheduler(this INovinoBuilder builder)
  {
    
    builder.Services.AddQuartz(q =>
    {
      var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x=>x.IsAssignableTo(typeof(IJob)));
      foreach (var type in types)
      {
        q.AddJob(type, new JobKey(type.Name));
        
      }
      q.AddTrigger(c =>
      {
        foreach (var type in types)
        {
          c.ForJob(type.Name).StartNow().WithSimpleSchedule(v => v.Build());
        }
        
      });
    });

    builder.Services.AddQuartzHostedService(options =>
    {
      // when shutting down we want jobs to complete gracefully
      options.WaitForJobsToComplete = true;
    });
    return builder;
  }
}
