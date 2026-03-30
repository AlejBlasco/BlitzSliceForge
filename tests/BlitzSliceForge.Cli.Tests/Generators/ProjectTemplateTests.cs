using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Templates.Project;

namespace BlitzSliceForge.Cli.Tests.Generators;

/// <summary>
/// Unit tests for <see cref="ProjectTemplate"/>.
/// Verifies that <see cref="ProjectTemplate.GetAvailableProjects"/> returns the expected
/// project definitions with correct names, directories and test-project flags.
/// </summary>
public class ProjectTemplateTests
{
    private static GenerationOptions BuildOptions(string name = "MyApp", string framework = "net9.0", string output = @"C:\Projects\MyApp")
        => new() { SolutionName = name, Framework = framework, OutputDirectory = output };

    // ── count ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Should return exactly seven projects (four source + three test).
    /// </summary>
    [Fact]
    public void GetAvailableProjects_ReturnsSevenProjects()
    {
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions());
        projects.Should().HaveCount(7);
    }

    // ── source projects ───────────────────────────────────────────────────────

    /// <summary>
    /// The four source projects should not be flagged as test projects.
    /// </summary>
    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Infrastructure")]
    [InlineData("Web")]
    public void GetAvailableProjects_SourceProjects_AreNotTestProjects(string suffix)
    {
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions());
        var match = projects.Where(p => p.Suffix == suffix && !p.IsTestProject).ToList();
        match.Should().ContainSingle($"expected exactly one non-test '{suffix}' project");
    }

    /// <summary>
    /// Source projects should reside under the <c>src</c> sub-folder.
    /// </summary>
    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Infrastructure")]
    [InlineData("Web")]
    public void GetAvailableProjects_SourceProjects_ProjectDirectoryContainsSrc(string suffix)
    {
        const string output = @"C:\Projects\MyApp";
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions(output: output));
        var match = projects.First(p => p.Suffix == suffix && !p.IsTestProject);
        match.ProjectDirectory.Should().Be(Path.Combine(output, "src"));
    }

    // ── test projects ─────────────────────────────────────────────────────────

    /// <summary>
    /// The three test projects should be flagged as test projects.
    /// </summary>
    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Infrastructure")]
    public void GetAvailableProjects_TestProjects_AreTestProjects(string suffix)
    {
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions());
        var match = projects.Where(p => p.Suffix == suffix && p.IsTestProject).ToList();
        match.Should().ContainSingle($"expected exactly one test '{suffix}' project");
    }

    /// <summary>
    /// Test projects should reside under the <c>tests</c> sub-folder.
    /// </summary>
    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Infrastructure")]
    public void GetAvailableProjects_TestProjects_ProjectDirectoryContainsTests(string suffix)
    {
        const string output = @"C:\Projects\MyApp";
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions(output: output));
        var match = projects.First(p => p.Suffix == suffix && p.IsTestProject);
        match.ProjectDirectory.Should().Be(Path.Combine(output, "tests"));
    }

    // ── framework propagation ─────────────────────────────────────────────────

    /// <summary>
    /// All projects should carry the framework value from <see cref="GenerationOptions"/>.
    /// </summary>
    [Theory]
    [InlineData("net8.0")]
    [InlineData("net9.0")]
    public void GetAvailableProjects_AllProjects_PropagateFramework(string framework)
    {
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions(framework: framework));
        projects.Should().AllSatisfy(p => p.Framework.Should().Be(framework));
    }

    // ── Blazor options ────────────────────────────────────────────────────────

    /// <summary>
    /// The Web (Blazor) project should carry the <c>--interactivity Auto</c> option.
    /// </summary>
    [Fact]
    public void GetAvailableProjects_BlazorProject_HasInteractivityAutoOption()
    {
        var projects = ProjectTemplate.GetAvailableProjects(BuildOptions());
        var blazor = projects.First(p => p.Suffix == "Web");
        blazor.Options.Should().Be("--interactivity Auto");
    }
}
