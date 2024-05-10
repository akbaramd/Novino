using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Novino.Abstractions.Novino.Abstractions;

public interface INovinApplicationBuilder
{
  IServiceCollection Services { get; }
  IConfiguration Configuration { get; }
  bool TryRegister(string name);
  void AddBuildAction(Action<IServiceProvider> execute);
  void AddInitializer(IInitializer initializer);
  void AddInitializer<TInitializer>() where TInitializer : IInitializer;
  
  public INovinApplication Build();
}
