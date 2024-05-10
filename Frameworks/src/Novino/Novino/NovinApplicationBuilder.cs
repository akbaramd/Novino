using System.Collections.Concurrent;
using Novino.Abstractions.Novino.Abstractions;
using Novino.Microsoft.AspNetCore.Builder;

namespace Novino.Novino;

public class NovinApplicationBuilder: INovinApplicationBuilder
{
  private readonly WebApplicationBuilder _applicationBuilder;
  private readonly ConcurrentDictionary<string, bool> _registry = new();
  private readonly List<Action<IServiceProvider>> _buildActions = [];
  private readonly IServiceCollection _services;
  IServiceCollection INovinApplicationBuilder.Services => _services;
        
  public IConfiguration Configuration { get; }

  internal NovinApplicationBuilder(WebApplicationBuilder applicationBuilder)
  {
    _applicationBuilder = applicationBuilder;
    _buildActions = new List<Action<IServiceProvider>>();
    _services = applicationBuilder.Services;
    _services.AddSingleton<IStartupInitializer>(new StartupInitializer());
    Configuration = applicationBuilder.Configuration;
  }


  public bool TryRegister(string name) => _registry.TryAdd(name, true);

  public void AddBuildAction(Action<IServiceProvider> execute)
    => _buildActions.Add(execute);

  public void AddInitializer(IInitializer initializer)
    => AddBuildAction(sp =>
    {
      var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
      startupInitializer.AddInitializer(initializer);
    });

  public void AddInitializer<TInitializer>() where TInitializer : IInitializer
    => AddBuildAction(sp =>
    {
      var initializer = sp.GetRequiredService<TInitializer>();
      var startupInitializer = sp.GetRequiredService<IStartupInitializer>();
      startupInitializer.AddInitializer(initializer);
    });

  public INovinApplication Build()
  {
    var application = _applicationBuilder.Build();
    var initializer = application.Services.CreateScope().ServiceProvider.GetRequiredService<IStartupInitializer>();
    _buildActions.ForEach(a => a(application.Services));
    initializer.InitializeAsync();
    return new NovinApplication(application);
  }
}
