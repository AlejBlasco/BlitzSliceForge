using BlitzSliceForge.Cli.Enums;

namespace BlitzSliceForge.Cli.Tests.Enums;

/// <summary>
/// Unit tests for <see cref="AvailableFrameworksEnumExtensions.ToFrameworkString"/>.
/// Verifies that each enum value maps to the correct TFM string and that unknown
/// values throw an <see cref="ArgumentOutOfRangeException"/>.
/// </summary>
public class AvailableFrameworksEnumExtensionsTests
{
    /// <summary>
    /// <see cref="AvailableFrameworksEnum.net8"/> should map to the TFM "net8.0".
    /// </summary>
    [Fact]
    public void ToFrameworkString_Net8_ReturnsNet8Point0()
    {
        // Arrange
        var framework = AvailableFrameworksEnum.net8;

        // Act
        var result = framework.ToFrameworkString();

        // Assert
        result.Should().Be("net8.0");
    }

    /// <summary>
    /// <see cref="AvailableFrameworksEnum.net9"/> should map to the TFM "net9.0".
    /// </summary>
    [Fact]
    public void ToFrameworkString_Net9_ReturnsNet9Point0()
    {
        // Arrange
        var framework = AvailableFrameworksEnum.net9;

        // Act
        var result = framework.ToFrameworkString();

        // Assert
        result.Should().Be("net9.0");
    }

    /// <summary>
    /// An enum value outside the defined range should throw <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    public void ToFrameworkString_OutOfRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var unknownFramework = (AvailableFrameworksEnum)999;

        // Act
        var act = () => unknownFramework.ToFrameworkString();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// All defined enum values must produce a non-null, non-empty TFM string.
    /// </summary>
    [Theory]
    [InlineData(AvailableFrameworksEnum.net8)]
    [InlineData(AvailableFrameworksEnum.net9)]
    public void ToFrameworkString_DefinedValues_ReturnsNonEmptyString(AvailableFrameworksEnum framework)
    {
        // Act
        var result = framework.ToFrameworkString();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}
