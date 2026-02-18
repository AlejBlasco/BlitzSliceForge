namespace BlitzSliceForge.Cli.Services;

public class DotNetCliService
{
    public async Task RunAsync(string arguments, string workingDir, CancellationToken ct = default)
    {
        //var psi = new System.Diagnostics.ProcessStartInfo("dotnet", arguments)
        //{
        //    WorkingDirectory = workingDir,
        //    RedirectStandardOutput = true,
        //    RedirectStandardError = true,
        //    UseShellExecute = false,
        //};

        //using var process = System.Diagnostics.Process.Start(psi)!;
        //await process.WaitForExitAsync(ct);

        //if (process.ExitCode != 0)
        //{
        //    var error = await process.StandardError.ReadToEndAsync();
        //    throw new Exception($"Error executing: dotnet 'dotnet {arguments}': {error}");
        //}
        await TryRunAsync(arguments, workingDir, ct);
    }

    public async Task<DotNetCliResponse> TryRunAsync(string arguments, string workingDir, CancellationToken ct = default)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("dotnet", arguments)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = System.Diagnostics.Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();

        var response = new DotNetCliResponse(process.ExitCode, output);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"Error executing: dotnet 'dotnet {arguments}': {error}");
        }

        return response;
    }
}


public class DotNetCliResponse
{
    public int ExitCode { get; set; }

    private readonly string rawOutput;
    public string RawOutput => rawOutput;

    public List<string> GetOutputLines()
    {
        if (string.IsNullOrEmpty(rawOutput))
            return new List<string>();
        else
            return rawOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public DotNetCliResponse(int exitCode, string rawOutput)
    {
        ExitCode = exitCode;
        this.rawOutput = rawOutput;
    }
}