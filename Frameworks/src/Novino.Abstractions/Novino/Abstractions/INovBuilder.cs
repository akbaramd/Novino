using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Novino.Abstractions.Novino.Abstractions;

public interface INovBuilder
{
  IServiceCollection Services { get; }
  IConfiguration Configuration { get; }
  bool TryRegister(string name);
  void AddBuildAction(Action<IServiceProvider> execute);
  void AddInitializer(INovInitializer novInitializer);
  void AddInitializer<TInitializer>() where TInitializer : INovInitializer;
  
  public INovinoApplication Build();
}
