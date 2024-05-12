using Novino.Abstractions;
using Quartz;

namespace Novino;

public static class NovinSchedulerExtensions
{
  public static INovinoBuilder AddQuartzScheduler(this INovinoBuilder builder , Action<IServiceCollectionQuartzConfigurator> configurator)
  {
    builder.Services.AddQuartz(configurator);

    builder.Services.AddQuartzHostedService(options =>
    {
      options.WaitForJobsToComplete = true;
    });
    return builder;
  }
}
