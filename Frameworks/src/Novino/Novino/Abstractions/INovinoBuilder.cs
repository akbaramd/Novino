using Microsoft.Extensions.DependencyInjection;

namespace Novino.Abstractions;

public interface INovinoBuilder
{
  NovinoServiceOptions ServiceOptions { get; }
    IServiceCollection Services { get; }
    bool TryRegister(string name);
    INovinoBuilder Initialize(Action<IServiceProvider> execute);
    INovinoBuilder Configure<TOptions> (Action<TOptions> configure)  where TOptions : class;
    INovinoBuilder AddInitializer (INovInitializer novInitializer);
    INovinoBuilder AddInitializer<TInitializer>() where TInitializer : INovInitializer;

    public INovinoApplication Build();
}