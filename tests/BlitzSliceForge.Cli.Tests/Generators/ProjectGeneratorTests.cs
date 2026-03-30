using BlitzSliceForge.Cli.Generators;
using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Services;

namespace BlitzSliceForge.Cli.Tests.Generators;

/// <summary>
/// Unit tests for <see cref="ProjectGenerator"/>.
/// Uses a mock of <see cref="IDotNetCliService"/> to avoid spawning real dotnet processes.
/// </summary>
public class ProjectGeneratorTests : IDisposable
{
    private readonly string tempDir;

    public ProjectGeneratorTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), $"bsf-gen-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }

    // ── constructor ───────────────────────────────────────────────────────────

    /// <summary>
    /// Constructor should throw <see cref="ArgumentNullException"/> when the CLI service is null.
    /// </summary>
    [Fact]
    public void Constructor_NullCliService_ThrowsArgumentNullException()
    {
        var act = () => new ProjectGenerator(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("cliService");
    }

    // ── CreateProjectAsync ────────────────────────────────────────────────────

    /// <summary>
    /// <see cref="ProjectGenerator.CreateProjectAsync"/> should call <see cref="IDotNetCliService.RunAsync"/>
    /// at least twice: once to scaffold the project and once to add it to the solution.
    /// </summary>
    [Fact]
    public async Task CreateProjectAsync_CallsCliService_AtLeastTwice()
    {
        // Arrange
        var mock = new Mock<IDotNetCliService>();
        mock.Setup(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new ProjectGenerator(mock.Object);

        var options = new ProjectGenerationOptions(
            solutionName: "TestApp",
            suffix: "Domain",
            template: "classlib",
            targetDirectory: tempDir,
            framework: "net9.0",
            isTestProject: false);

        // Act
        await sut.CreateProjectAsync(options);

        // Assert
        mock.Verify(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }

    /// <summary>
    /// <see cref="ProjectGenerator.CreateProjectAsync"/> should ensure the project directory
    /// exists before invoking the CLI.
    /// </summary>
    [Fact]
    public async Task CreateProjectAsync_CreatesProjectDirectoryIfMissing()
    {
        // Arrange
        var mock = new Mock<IDotNetCliService>();
        mock.Setup(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new ProjectGenerator(mock.Object);

        var nestedTarget = Path.Combine(tempDir, "nested");
        var options = new ProjectGenerationOptions(
            solutionName: "TestApp",
            suffix: "Domain",
            template: "classlib",
            targetDirectory: nestedTarget,
            framework: "net9.0",
            isTestProject: false);

        // Act
        await sut.CreateProjectAsync(options);

        // Assert — ProjectDirectory (src sub-folder) must exist after the call
        Directory.Exists(options.ProjectDirectory).Should().BeTrue();
    }

    // ── AddProjectReferenceAsync ──────────────────────────────────────────────

    /// <summary>
    /// <see cref="ProjectGenerator.AddProjectReferenceAsync"/> should call
    /// <see cref="IDotNetCliService.RunAsync"/> when both project files exist.
    /// </summary>
    [Fact]
    public async Task AddProjectReferenceAsync_BothFilesExist_CallsCliService()
    {
        // Arrange — create two real (empty) .csproj placeholder files
        var csprojA = Path.Combine(tempDir, "A.csproj");
        var csprojB = Path.Combine(tempDir, "B.csproj");
        await File.WriteAllTextAsync(csprojA, "<Project />");
        await File.WriteAllTextAsync(csprojB, "<Project />");

        var mock = new Mock<IDotNetCliService>();
        mock.Setup(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new ProjectGenerator(mock.Object);

        // Act
        await sut.AddProjectReferenceAsync(tempDir, csprojA, csprojB);

        // Assert
        mock.Verify(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// <see cref="ProjectGenerator.AddProjectReferenceAsync"/> should NOT call
    /// <see cref="IDotNetCliService.RunAsync"/> when the referencing project does not exist.
    /// </summary>
    [Fact]
    public async Task AddProjectReferenceAsync_ReferencingFileMissing_DoesNotCallCliService()
    {
        // Arrange
        var missingCsproj = Path.Combine(tempDir, "Missing.csproj");
        var existingCsproj = Path.Combine(tempDir, "Existing.csproj");
        await File.WriteAllTextAsync(existingCsproj, "<Project />");

        var mock = new Mock<IDotNetCliService>();
        var sut = new ProjectGenerator(mock.Object);

        // Act
        await sut.AddProjectReferenceAsync(tempDir, missingCsproj, existingCsproj);

        // Assert
        mock.Verify(m => m.RunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
