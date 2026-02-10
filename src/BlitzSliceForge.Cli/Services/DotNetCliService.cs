namespace BlitzSliceForge.Cli.Services;

public class DotNetCliService
{
    public async Task RunAsync(string arguments, string workingDir, CancellationToken ct = default)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("dotnet", arguments)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = System.Diagnostics.Process.Start(psi)!;
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"Error executing: dotnet 'dotnet {arguments}': {error}");
        }
    }
}
