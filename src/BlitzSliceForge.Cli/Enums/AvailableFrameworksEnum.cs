namespace BlitzSliceForge.Cli.Enums;
public enum AvailableFrameworksEnum
{
    net8,
    net9,
}

public static class AvailableFrameworksEnumExtensions
{
    public static string ToFrameworkString(this AvailableFrameworksEnum framework)
    {
        return framework switch
        {
            AvailableFrameworksEnum.net8 => "net8.0",
            AvailableFrameworksEnum.net9 => "net9.0",
            _ => throw new ArgumentOutOfRangeException(nameof(framework))
        };
    }
}


