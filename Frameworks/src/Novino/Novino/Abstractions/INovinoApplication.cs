namespace Novino.Abstractions;

public interface INovinoApplication
{

    IApplicationBuilder ApplicationBuilder { get; }
    public void Run();
    public Task RunAsync();
}