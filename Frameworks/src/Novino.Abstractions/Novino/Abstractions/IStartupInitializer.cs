namespace Novino.Abstractions.Novino.Abstractions;

public interface IStartupInitializer : IInitializer
{
  void AddInitializer(IInitializer initializer);
}