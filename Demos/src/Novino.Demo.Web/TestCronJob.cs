using Quartz;

namespace Novino.Demo.Web;

[DisallowConcurrentExecution]
public class TestCronJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Cron :)");
        return Task.CompletedTask;
    }
}