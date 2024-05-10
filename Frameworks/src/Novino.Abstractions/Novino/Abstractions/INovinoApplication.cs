namespace Novino.Abstractions.Novino.Abstractions;

public interface INovinoApplication
{
  public void Run();
  public Task RunAsync();
}
