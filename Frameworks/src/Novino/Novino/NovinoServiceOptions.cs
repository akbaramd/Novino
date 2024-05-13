namespace Novino;

public class NovinoServiceOptions
{
  public required string Id { get; set; } 
  public required string Name { get; set; } 
  public required string Version { get; set; }
  public string Description { get; set; } = string.Empty;
}
