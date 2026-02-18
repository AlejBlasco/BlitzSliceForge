namespace BlitzSliceForge.Cli.Services;

public class SdkCheckerService
{
    private readonly DotNetCliService cliService;

    public SdkCheckerService(DotNetCliService cliService)
    {
        this.cliService = cliService
            ?? throw new ArgumentNullException(nameof(cliService));
    }

    public async Task<bool> HasSdkAsync(string targetFramework, CancellationToken ct = default)
    {
        var sdks = await GetInstalledSdkVersionsAsync(ct);
        var majorMinor = targetFramework.Replace("net", "");

        return sdks.Any(v => v.StartsWith(majorMinor));
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
