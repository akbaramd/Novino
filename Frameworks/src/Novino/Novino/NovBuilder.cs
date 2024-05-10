using System.Collections.Concurrent;
using Novino.Abstractions.Novino.Abstractions;
using Novino.Microsoft.AspNetCore.Builder;

namespace Novino.Novino;

public class NovBuilder: INovBuilder
{
  private readonly WebApplicationBuilder _applicationBuilder;
  private readonly ConcurrentDictionary<string, bool> _registry = new();
  private readonly List<Action<IServiceProvider>> _buildActions = [];
  private readonly IServiceCollection _services;
  IServiceCollection INovBuilder.Services => _services;
        
  public IConfiguration Configuration { get; }

  internal NovBuilder(WebApplicationBuilder applicationBuilder)
  {
    _applicationBuilder = applicationBuilder;
    _buildActions = new List<Action<IServiceProvider>>();
    _services = applicationBuilder.Services;
    _services.AddSingleton<INovStartupInitializer>(new NovStartupInitializer());
    Configuration = applicationBuilder.Configuration;
  }


  public bool TryRegister(string name) => _registry.TryAdd(name, true);

  public void AddBuildAction(Action<IServiceProvider> execute)
    => _buildActions.Add(execute);

  public void AddInitializer(INovInitializer novInitializer)
    => AddBuildAction(sp =>
    {
      var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
      startupInitializer.AddInitializer(novInitializer);
    });

  public void AddInitializer<TInitializer>() where TInitializer : INovInitializer
    => AddBuildAction(sp =>
    {
      var initializer = sp.GetRequiredService<TInitializer>();
      var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
      startupInitializer.AddInitializer(initializer);
    });

  public INovinoApplication Build()
  {
    var application = _applicationBuilder.Build();
    var initializer = application.Services.CreateScope().ServiceProvider.GetRequiredService<INovStartupInitializer>();
    _buildActions.ForEach(a => a(application.Services));
    initializer.InitializeAsync();
    return new NovinoApplication(application);
  }
}
