namespace Novino.Abstractions;

public interface INovinoApplication
{

  NovinoServiceOptions ServiceOptions { get; }
    IApplicationBuilder ApplicationBuilder { get; }
    public void Run();
    public Task RunAsync();
}