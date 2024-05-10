using Novino.Abstractions.Novino.Abstractions;

namespace Novino.Novino;


public class NovStartupInitializer : INovStartupInitializer
{
  private readonly IList<INovInitializer> _initializers = new List<INovInitializer>();

  public void AddInitializer(INovInitializer novInitializer)
  {
    if (_initializers.Contains(novInitializer))
    {
      return;
    }

    _initializers.Add(novInitializer);

  }

  public async Task InitializeAsync()
  {
    foreach (var initializer in _initializers)
    {
      await initializer.InitializeAsync();
    }
  }
}