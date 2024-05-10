namespace Novino.Abstractions.Novino.Abstractions;

public interface INovStartupInitializer : INovInitializer
{
  void AddInitializer(INovInitializer novInitializer);
}