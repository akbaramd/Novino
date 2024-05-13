using System.Collections.Concurrent;
using Novino.Abstractions;

namespace Novino;

public class NovinoBuilder : INovinoBuilder
{
    private readonly WebApplicationBuilder _applicationBuilder;
    private readonly List<Action<IServiceProvider>> _buildActions = [];
    private readonly ConcurrentDictionary<string, bool> _registry = new();


    internal NovinoBuilder(WebApplicationBuilder applicationBuilder, NovinoServiceOptions serviceOptions)
    {
        _applicationBuilder = applicationBuilder;
        ServiceOptions = serviceOptions;
        Services = applicationBuilder.Services;
        _buildActions = new List<Action<IServiceProvider>>();
        applicationBuilder.Services.AddSingleton<INovStartupInitializer>(new NovStartupInitializer());
    }

    public NovinoServiceOptions ServiceOptions { get; }
    public IServiceCollection Services { get; }

    public bool TryRegister(string name)
    {
        return _registry.TryAdd(name, true);
    }

    public void Initialize(Action<IServiceProvider> execute)
    {
        _buildActions.Add(execute);
    }

    public void Configure<TOptions>(Action<TOptions> configure) where TOptions : class
    {
      _applicationBuilder.Services.Configure<TOptions>(configure);
    }
    public void AddInitializer(INovInitializer novInitializer)
    {
        Initialize(sp =>
        {
            var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
            startupInitializer.AddInitializer(novInitializer);
        });
    }

    public void AddInitializer<TInitializer>() where TInitializer : INovInitializer
    {
        Initialize(sp =>
        {
            var initializer = sp.GetRequiredService<TInitializer>();
            var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });
    }

    public INovinoApplication Build()
    {
        var application = _applicationBuilder.Build();
        var startupInitializer = application.Services.GetRequiredService<INovStartupInitializer>();
        _buildActions.ForEach(a => a(application.Services));
        startupInitializer.InitializeAsync().GetAwaiter().GetResult();
        application.UseRouting();
        return new NovinoWebApplication(application,ServiceOptions);
    }
}