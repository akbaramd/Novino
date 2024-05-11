using System.Collections.Concurrent;
using Novino.Abstractions;

namespace Novino;

public class NovinoApplicationBuilder : INovinoApplicationBuilder
{
    private readonly WebApplicationBuilder _applicationBuilder;
    private readonly List<Action<IServiceProvider>> _buildActions = [];
    private readonly ConcurrentDictionary<string, bool> _registry = new();


    internal NovinoApplicationBuilder(WebApplicationBuilder applicationBuilder)
    {
        _applicationBuilder = applicationBuilder;
        Services = applicationBuilder.Services;
        _buildActions = new List<Action<IServiceProvider>>();
        applicationBuilder.Services.AddSingleton<INovStartupInitializer>(new NovStartupInitializer());
    }


    public IServiceCollection Services { get; }

    public bool TryRegister(string name)
    {
        return _registry.TryAdd(name, true);
    }

    public void AddBuildAction(Action<IServiceProvider> execute)
    {
        _buildActions.Add(execute);
    }

    public void AddInitializer(INovInitializer novInitializer)
    {
        AddBuildAction(sp =>
        {
            var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
            startupInitializer.AddInitializer(novInitializer);
        });
    }

    public void AddInitializer<TInitializer>() where TInitializer : INovInitializer
    {
        AddBuildAction(sp =>
        {
            var initializer = sp.GetRequiredService<TInitializer>();
            var startupInitializer = sp.GetRequiredService<INovStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        });
    }

    public INovinoApplication Build()
    {
        var application = _applicationBuilder.Build();
        _buildActions.ForEach(a => a(application.Services));
        return new NovinoWebApplication(application);
    }
}