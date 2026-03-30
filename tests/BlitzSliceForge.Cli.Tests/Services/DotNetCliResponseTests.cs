using BlitzSliceForge.Cli.Services;

namespace BlitzSliceForge.Cli.Tests.Services;

/// <summary>
/// Unit tests for <see cref="DotNetCliResponse"/>.
/// Verifies constructor assignment and the line-splitting logic of
/// <see cref="DotNetCliResponse.GetOutputLines"/>.
/// </summary>
public class DotNetCliResponseTests
{
    /// <summary>
    /// Constructor should correctly assign <see cref="DotNetCliResponse.ExitCode"/>
    /// and <see cref="DotNetCliResponse.RawOutput"/>.
    /// </summary>
    [Fact]
    public void Constructor_AssignsProperties()
    {
        // Arrange & Act
        var response = new DotNetCliResponse(0, "some output");

        // Assert
        response.ExitCode.Should().Be(0);
        response.RawOutput.Should().Be("some output");
    }

    /// <summary>
    /// <see cref="DotNetCliResponse.GetOutputLines"/> on empty output should return an empty list.
    /// </summary>
    [Fact]
    public void GetOutputLines_EmptyOutput_ReturnsEmptyList()
    {
        // Arrange
        var response = new DotNetCliResponse(0, string.Empty);

        // Act
        var lines = response.GetOutputLines();

        // Assert
        lines.Should().BeEmpty();
    }

    /// <summary>
    /// <see cref="DotNetCliResponse.GetOutputLines"/> should split Unix line endings correctly.
    /// </summary>
    [Fact]
    public void GetOutputLines_UnixLineEndings_ReturnsSplitLines()
    {
        // Arrange
        var response = new DotNetCliResponse(0, "line1\nline2\nline3");

        // Act
        var lines = response.GetOutputLines();

        // Assert
        lines.Should().HaveCount(3)
             .And.ContainInOrder("line1", "line2", "line3");
    }

    /// <summary>
    /// <see cref="DotNetCliResponse.GetOutputLines"/> should split Windows (CRLF) line endings correctly.
    /// </summary>
    [Fact]
    public void GetOutputLines_WindowsLineEndings_ReturnsSplitLines()
    {
        // Arrange
        var response = new DotNetCliResponse(0, "line1\r\nline2\r\nline3");

        // Act
        var lines = response.GetOutputLines();

        // Assert
        lines.Should().HaveCount(3)
             .And.ContainInOrder("line1", "line2", "line3");
    }

    /// <summary>
    /// <see cref="DotNetCliResponse.GetOutputLines"/> should skip blank lines.
    /// </summary>
    [Fact]
    public void GetOutputLines_BlankLines_AreIgnored()
    {
        // Arrange
        var response = new DotNetCliResponse(0, "line1\n\nline2\n\n");

        // Act
        var lines = response.GetOutputLines();

        // Assert
        lines.Should().HaveCount(2)
             .And.ContainInOrder("line1", "line2");
    }

    /// <summary>
    /// <see cref="DotNetCliResponse.GetOutputLines"/> on null raw output should return an empty list.
    /// </summary>
    [Fact]
    public void GetOutputLines_NullOutput_ReturnsEmptyList()
    {
        // Arrange
        var response = new DotNetCliResponse(0, null!);

        // Act
        var lines = response.GetOutputLines();

        // Assert
        lines.Should().BeEmpty();
    }
}
