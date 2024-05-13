using Microsoft.Extensions.DependencyInjection;

namespace Novino.Abstractions;

public interface INovinoBuilder
{
  NovinoServiceOptions ServiceOptions { get; }
    IServiceCollection Services { get; }
    bool TryRegister(string name);
    void Initialize(Action<IServiceProvider> execute);
    void Configure<TOptions> (Action<TOptions> configure)  where TOptions : class;
    void AddInitializer (INovInitializer novInitializer);
    void AddInitializer<TInitializer>() where TInitializer : INovInitializer;

    public INovinoApplication Build();
}