using Microsoft.Extensions.DependencyInjection;

namespace Novino.Abstractions;

public interface INovinoApplicationBuilder
{
    IServiceCollection Services { get; }
    bool TryRegister(string name);
    void AddBuildAction(Action<IServiceProvider> execute);
    void AddInitializer(INovInitializer novInitializer);
    void AddInitializer<TInitializer>() where TInitializer : INovInitializer;

    public INovinoApplication Build();
}