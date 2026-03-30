using BlitzSliceForge.Cli.Models;

namespace BlitzSliceForge.Cli.Tests.Models;

/// <summary>
/// Unit tests for <see cref="GenerationOptions"/>.
/// Verifies default values and property assignment.
/// </summary>
public class GenerationOptionsTests
{
    /// <summary>
    /// Default constructor should initialise string properties to <see cref="string.Empty"/>.
    /// </summary>
    [Fact]
    public void DefaultConstructor_StringProperties_AreEmpty()
    {
        // Act
        var options = new GenerationOptions();

        // Assert
        options.SolutionName.Should().BeEmpty();
        options.Framework.Should().BeEmpty();
        options.OutputDirectory.Should().BeNull();
    }

    /// <summary>
    /// Properties should reflect the values assigned to them.
    /// </summary>
    [Fact]
    public void Properties_WhenAssigned_ReturnCorrectValues()
    {
        // Arrange & Act
        var options = new GenerationOptions
        {
            SolutionName = "MySolution",
            Framework = "net9.0",
            OutputDirectory = @"C:\Projects\MySolution"
        };

        // Assert
        options.SolutionName.Should().Be("MySolution");
        options.Framework.Should().Be("net9.0");
        options.OutputDirectory.Should().Be(@"C:\Projects\MySolution");
    }
}
