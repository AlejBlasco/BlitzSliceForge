using BlitzSliceForge.Cli.Services;

namespace BlitzSliceForge.Cli.Tests.Services;

/// <summary>
/// Unit tests for <see cref="SdkCheckerService"/>.
/// Uses a mock of <see cref="IDotNetCliService"/> to avoid spawning real dotnet processes.
/// </summary>
public class SdkCheckerServiceTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a mock <see cref="IDotNetCliService"/> whose <c>TryRunAsync</c> returns
    /// a successful response containing the given SDK version lines.
    /// </summary>
    private static Mock<IDotNetCliService> BuildMockCli(params string[] sdkLines)
    {
        var rawOutput = string.Join("\n", sdkLines);
        var response = new DotNetCliResponse(0, rawOutput);

        var mock = new Mock<IDotNetCliService>();
        mock.Setup(m => m.TryRunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        return mock;
    }

    // ── constructor ───────────────────────────────────────────────────────────

    /// <summary>
    /// Constructor should throw <see cref="ArgumentNullException"/> when the CLI service is null.
    /// </summary>
    [Fact]
    public void Constructor_NullCliService_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new SdkCheckerService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("cliService");
    }

    // ── ValidateFrameworkAsync ────────────────────────────────────────────────

    /// <summary>
    /// Should return <c>true</c> when the required SDK version is present in the output.
    /// </summary>
    [Fact]
    public async Task ValidateFrameworkAsync_SdkPresent_ReturnsTrue()
    {
        // Arrange
        var mock = BuildMockCli("9.0.100 [C:\\Program Files\\dotnet\\sdk]");
        var sut = new SdkCheckerService(mock.Object);

        // Act
        var result = await sut.ValidateFrameworkAsync("net9.0", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Should return <c>false</c> when no SDK matching the required version is installed.
    /// </summary>
    [Fact]
    public async Task ValidateFrameworkAsync_SdkNotPresent_ReturnsFalse()
    {
        // Arrange
        var mock = BuildMockCli("8.0.100 [C:\\Program Files\\dotnet\\sdk]");
        var sut = new SdkCheckerService(mock.Object);

        // Act
        var result = await sut.ValidateFrameworkAsync("net9.0", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Should return <c>false</c> when the SDK list is empty.
    /// </summary>
    [Fact]
    public async Task ValidateFrameworkAsync_EmptySdkList_ReturnsFalse()
    {
        // Arrange
        var mock = BuildMockCli();
        var sut = new SdkCheckerService(mock.Object);

        // Act
        var result = await sut.ValidateFrameworkAsync("net9.0", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Should return <c>true</c> when a net8 SDK is installed and net8.0 is requested.
    /// </summary>
    [Fact]
    public async Task ValidateFrameworkAsync_Net8SdkPresent_ReturnsTrue()
    {
        // Arrange
        var mock = BuildMockCli("8.0.404 [C:\\Program Files\\dotnet\\sdk]");
        var sut = new SdkCheckerService(mock.Object);

        // Act
        var result = await sut.ValidateFrameworkAsync("net8.0", CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Should call <see cref="IDotNetCliService.TryRunAsync"/> exactly once per validation call.
    /// </summary>
    [Fact]
    public async Task ValidateFrameworkAsync_CallsCliServiceOnce()
    {
        // Arrange
        var mock = BuildMockCli("9.0.100 [C:\\Program Files\\dotnet\\sdk]");
        var sut = new SdkCheckerService(mock.Object);

        // Act
        await sut.ValidateFrameworkAsync("net9.0", CancellationToken.None);

        // Assert
        mock.Verify(m => m.TryRunAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
