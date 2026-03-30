namespace BlitzSliceForge.Cli.Services;

/// <summary>
/// Runs dotnet CLI commands as child processes.
/// </summary>
public class DotNetCliService : IDotNetCliService
{
    /// <inheritdoc/>
    public async Task RunAsync(string arguments, string workingDir, CancellationToken ct = default)
    {
        await TryRunAsync(arguments, workingDir, ct);
    }

    /// <inheritdoc/>
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

/// <summary>
/// Holds the result of a dotnet CLI invocation.
/// </summary>
public class DotNetCliResponse
{
    /// <summary>Gets the process exit code.</summary>
    public int ExitCode { get; set; }

    private readonly string rawOutput;

    /// <summary>Gets the raw standard output text.</summary>
    public string RawOutput => rawOutput;

    /// <summary>
    /// Splits <see cref="RawOutput"/> into non-empty lines, handling both
    /// Unix (<c>\n</c>) and Windows (<c>\r\n</c>) line endings.
    /// </summary>
    public List<string> GetOutputLines()
    {
        if (string.IsNullOrEmpty(rawOutput))
            return new List<string>();

        return rawOutput
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    /// <summary>Initialises a new <see cref="DotNetCliResponse"/>.</summary>
    public DotNetCliResponse(int exitCode, string rawOutput)
    {
        ExitCode = exitCode;
        this.rawOutput = rawOutput;
    }
}
