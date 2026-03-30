namespace BlitzSliceForge.Cli.Services;

/// <summary>
/// Abstraction over the dotnet CLI process runner.
/// Extracted to allow unit testing via mocking.
/// </summary>
public interface IDotNetCliService
{
    /// <summary>
    /// Runs a dotnet CLI command and throws on non-zero exit code.
    /// </summary>
    Task RunAsync(string arguments, string workingDir, CancellationToken ct = default);

    /// <summary>
    /// Runs a dotnet CLI command and returns the full response (exit code + output).
    /// </summary>
    Task<DotNetCliResponse> TryRunAsync(string arguments, string workingDir, CancellationToken ct = default);
}
