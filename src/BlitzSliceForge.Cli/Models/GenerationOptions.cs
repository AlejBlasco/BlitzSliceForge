namespace BlitzSliceForge.Cli.Models;

public class GenerationOptions
{
    public string SolutionName { get; set; } = string.Empty;
    public string? OutputDirectory { get; set; }
    public string Framework { get; set; } = "net8.0";
}
