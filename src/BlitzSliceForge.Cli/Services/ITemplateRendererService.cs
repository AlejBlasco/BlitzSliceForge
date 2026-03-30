namespace BlitzSliceForge.Cli.Services;

/// <summary>
/// Abstraction over the Scriban template renderer and file writer.
/// Extracted to allow unit testing via mocking.
/// </summary>
public interface ITemplateRendererService
{
    /// <summary>
    /// Renders a Scriban template file with the given model and writes the result to disk.
    /// </summary>
    Task RenderAndSaveAsync(string templatePath, string outputPath, object? model = null, CancellationToken ct = default);
}
