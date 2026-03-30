using BlitzSliceForge.Cli.Services;

namespace BlitzSliceForge.Cli.Tests.Services;

/// <summary>
/// Unit tests for <see cref="TemplateRendererService"/>.
/// Uses real temporary files to validate filesystem behaviour without mocking the OS.
/// All temporary files are cleaned up in <see cref="IDisposable.Dispose"/>.
/// </summary>
public class TemplateRendererServiceTests : IDisposable
{
    private readonly string tempDir;
    private readonly TemplateRendererService sut;

    /// <summary>
    /// Creates an isolated temp directory for each test.
    /// </summary>
    public TemplateRendererServiceTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"bsf-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        sut = new TemplateRendererService();
    }

    /// <summary>Removes the temp directory after each test.</summary>
    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private string TempPath(string fileName) => Path.Combine(tempDir, fileName);

    // ── non-existent template ─────────────────────────────────────────────────

    /// <summary>
    /// Should complete without throwing when the template file does not exist.
    /// </summary>
    [Fact]
    public async Task RenderAndSaveAsync_MissingTemplate_DoesNotThrow()
    {
        // Arrange
        var missingTemplate = TempPath("nonexistent.scriban");
        var outputPath = TempPath("output.txt");

        // Act
        var act = async () => await sut.RenderAndSaveAsync(missingTemplate, outputPath);

        // Assert
        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Should not create an output file when the template does not exist.
    /// </summary>
    [Fact]
    public async Task RenderAndSaveAsync_MissingTemplate_DoesNotCreateOutput()
    {
        // Arrange
        var missingTemplate = TempPath("nonexistent.scriban");
        var outputPath = TempPath("output.txt");

        // Act
        await sut.RenderAndSaveAsync(missingTemplate, outputPath);

        // Assert
        File.Exists(outputPath).Should().BeFalse();
    }

    // ── static template (no model) ────────────────────────────────────────────

    /// <summary>
    /// Should write the rendered content to disk when the template is valid and has no variables.
    /// </summary>
    [Fact]
    public async Task RenderAndSaveAsync_ValidStaticTemplate_WritesOutputFile()
    {
        // Arrange
        var templatePath = TempPath("static.scriban");
        var outputPath = TempPath("static-output.txt");
        await File.WriteAllTextAsync(templatePath, "Hello, world!");

        // Act
        await sut.RenderAndSaveAsync(templatePath, outputPath);

        // Assert
        File.Exists(outputPath).Should().BeTrue();
        var content = await File.ReadAllTextAsync(outputPath);
        content.Should().Be("Hello, world!");
    }

    // ── template with model ───────────────────────────────────────────────────

    /// <summary>
    /// Should interpolate model variables into the rendered output.
    /// </summary>
    [Fact]
    public async Task RenderAndSaveAsync_TemplateWithModel_InterpolatesVariables()
    {
        // Arrange
        var templatePath = TempPath("model.scriban");
        var outputPath = TempPath("model-output.txt");
        await File.WriteAllTextAsync(templatePath, "Solution: {{ solution_name }}");

        var model = new Dictionary<string, object> { { "solution_name", "MyApp" } };

        // Act
        await sut.RenderAndSaveAsync(templatePath, outputPath, model);

        // Assert
        var content = await File.ReadAllTextAsync(outputPath);
        content.Should().Be("Solution: MyApp");
    }

    // ── cancellation ─────────────────────────────────────────────────────────

    /// <summary>
    /// Should throw <see cref="OperationCanceledException"/> when the token is already cancelled.
    /// </summary>
    [Fact]
    public async Task RenderAndSaveAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var templatePath = TempPath("cancel.scriban");
        var outputPath = TempPath("cancel-output.txt");
        await File.WriteAllTextAsync(templatePath, "test");

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await sut.RenderAndSaveAsync(templatePath, outputPath, null, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
