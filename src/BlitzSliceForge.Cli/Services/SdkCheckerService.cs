namespace BlitzSliceForge.Cli.Services;

public class SdkCheckerService
{
    private readonly DotNetCliService cliService;

    public SdkCheckerService(DotNetCliService cliService)
    {
        this.cliService = cliService
            ?? throw new ArgumentNullException(nameof(cliService));
    }

    public async Task<bool> ValidateFrameworkAsync(string framework, CancellationToken ct)
    {
        var installedSdks = await GetInstalledSdkVersionsAsync(ct);

        var requiredMajorMinor = framework.Replace("net", ""); // net8.0 → "8.0"

        if (!installedSdks.Any(v => v.StartsWith(requiredMajorMinor + ".")))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: SDK .NET {framework} not installed.");
            Console.WriteLine($"Detected SDKs:");
            foreach (var sdk in installedSdks)
                Console.WriteLine($"  - {sdk}");

            Console.WriteLine($"\nTo use --framework {framework}, you need to install the corresponding SDK:");
            Console.WriteLine($"→ https://dotnet.microsoft.com/download/dotnet/{requiredMajorMinor}");
            Console.WriteLine("\nOnce installed, please rerun the command.\n");
            Console.ResetColor();

            return false;
        }

        return true;
    }

    private async Task<List<string>> GetInstalledSdkVersionsAsync(CancellationToken ct = default)
    {
        var sdks = new List<string>();

        var dotnetResponse = await cliService.TryRunAsync("--list-sdks", Environment.CurrentDirectory, ct);
        if (dotnetResponse.ExitCode == 0)
        {
            foreach (var line in dotnetResponse.GetOutputLines())
            {
                var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1 && parts[0].StartsWith("8.") || parts[0].StartsWith("9."))
                {
                    sdks.Add(parts[0]);
                }
            }
        }

        return sdks;
    }


}
