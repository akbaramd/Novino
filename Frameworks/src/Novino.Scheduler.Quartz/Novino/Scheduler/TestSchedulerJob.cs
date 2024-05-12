using Quartz;

namespace Novino.Scheduler;

public class TestSchedulerJob : IJob
{
    
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Work");
        return Task.CompletedTask;
    }
}