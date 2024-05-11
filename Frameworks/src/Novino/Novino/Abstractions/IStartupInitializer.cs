namespace Novino.Abstractions;

public interface INovStartupInitializer : INovInitializer
{
    void AddInitializer(INovInitializer novInitializer);
}