using Novino.Abstractions.Novino.Abstractions;

namespace Novino.Novino;


public class StartupInitializer : IStartupInitializer
{
  private readonly IList<IInitializer> _initializers = new List<IInitializer>();

  public void AddInitializer(IInitializer initializer)
  {
    if (_initializers.Contains(initializer))
    {
      return;
    }

    _initializers.Add(initializer);

  }

  public async Task InitializeAsync()
  {
    foreach (var initializer in _initializers)
    {
      await initializer.InitializeAsync();
    }
  }
}