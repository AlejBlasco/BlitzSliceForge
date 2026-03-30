using BlitzSliceForge.Cli.Models;

namespace BlitzSliceForge.Cli.Tests.Models;

/// <summary>
/// Unit tests for <see cref="ProjectGenerationOptions"/>.
/// Verifies computed properties: <see cref="ProjectGenerationOptions.ProjectName"/>,
/// <see cref="ProjectGenerationOptions.ProjectDirectory"/> and
/// <see cref="ProjectGenerationOptions.FullProjectPath"/>.
/// </summary>
public class ProjectGenerationOptionsTests
{
    private const string SolutionName = "MyApp";
    private const string TargetDir = @"C:\Projects\MyApp";
    private const string Framework = "net9.0";

    /// <summary>
    /// <see cref="ProjectGenerationOptions.ProjectName"/> should concatenate
    /// <c>SolutionName</c> and <c>Suffix</c> with a dot separator.
    /// </summary>
    [Fact]
    public void ProjectName_IsComposedOfSolutionNameAndSuffix()
    {
        // Arrange
        var options = new ProjectGenerationOptions(SolutionName, "Domain", "classlib", TargetDir, Framework);

        // Assert
        options.ProjectName.Should().Be("MyApp.Domain");
    }

    /// <summary>
    /// <see cref="ProjectGenerationOptions.ProjectDirectory"/> for a non-test project
    /// should be under the <c>src</c> sub-folder.
    /// </summary>
    [Fact]
    public void ProjectDirectory_NonTestProject_IsUnderSrc()
    {
        // Arrange
        var options = new ProjectGenerationOptions(SolutionName, "Application", "classlib", TargetDir, Framework, isTestProject: false);

        // Assert
        options.ProjectDirectory.Should().Be(Path.Combine(TargetDir, "src"));
    }

    /// <summary>
    /// <see cref="ProjectGenerationOptions.ProjectDirectory"/> for a test project
    /// should be under the <c>tests</c> sub-folder.
    /// </summary>
    [Fact]
    public void ProjectDirectory_TestProject_IsUnderTests()
    {
        // Arrange
        var options = new ProjectGenerationOptions(SolutionName, "Application.Tests", "xunit", TargetDir, Framework, isTestProject: true);

        // Assert
        options.ProjectDirectory.Should().Be(Path.Combine(TargetDir, "tests"));
    }

    /// <summary>
    /// <see cref="ProjectGenerationOptions.FullProjectPath"/> should resolve to
    /// <c>ProjectDirectory/ProjectName/ProjectName.csproj</c>.
    /// </summary>
    [Theory]
    [InlineData(false, "src")]
    [InlineData(true, "tests")]
    public void FullProjectPath_ReflectsProjectDirectoryAndName(bool isTest, string subFolder)
    {
        // Arrange
        var suffix = isTest ? "Domain.Tests" : "Domain";
        var options = new ProjectGenerationOptions(SolutionName, suffix, "classlib", TargetDir, Framework, isTestProject: isTest);

        var expectedPath = Path.Combine(TargetDir, subFolder, options.ProjectName, $"{options.ProjectName}.csproj");

        // Assert
        options.FullProjectPath.Should().Be(expectedPath);
    }

    /// <summary>
    /// The optional <c>Options</c> property should be null by default and hold the value when provided.
    /// </summary>
    [Fact]
    public void Options_DefaultIsNull_AndCanBeSet()
    {
        // Arrange
        var withoutOptions = new ProjectGenerationOptions(SolutionName, "Domain", "classlib", TargetDir, Framework);
        var withOptions = new ProjectGenerationOptions(SolutionName, "Domain", "classlib", TargetDir, Framework, options: "--no-restore");

        // Assert
        withoutOptions.Options.Should().BeNull();
        withOptions.Options.Should().Be("--no-restore");
    }
}
