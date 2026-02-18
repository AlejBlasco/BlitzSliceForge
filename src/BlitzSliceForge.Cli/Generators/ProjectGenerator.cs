using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Services;

namespace BlitzSliceForge.Cli.Generators;

public class ProjectGenerator
{
    private readonly DotNetCliService cliService;

    public ProjectGenerator(DotNetCliService cliService)
    {
        this.cliService = cliService
            ?? throw new ArgumentNullException(nameof(cliService));
    }

    public async Task CreateProjectAsync(ProjectGenerationOptions options, CancellationToken ct = default)
    {
        // Ensure the project directory exists
        if (!Directory.Exists(options.ProjectDirectory))
            Directory.CreateDirectory(options.ProjectDirectory);

        Console.WriteLine($"Generating and linking project: {options.ProjectName} in {options.ProjectDirectory} ...");

        // Generate the project using the specified template
        await cliService.RunAsync($"dotnet new {options.Template} -n {options.ProjectName} -f {options.Framework}", options.ProjectDirectory);

        // Remove the default Class1.cs / UnitTest1.cs file if it exists
        var defaultClassFilePath = Path.Combine(options.ProjectDirectory, options.ProjectName, "Class1.cs");
        if (File.Exists(defaultClassFilePath))
            File.Delete(defaultClassFilePath);

        defaultClassFilePath = Path.Combine(options.ProjectDirectory, options.ProjectName, "UnitTest1.cs");
        if (File.Exists(defaultClassFilePath))
            File.Delete(defaultClassFilePath);

        // Clean up any extra solution files that may have been created in the project directory (e.g., from nested templates)
        foreach (var solutionFile in Directory.GetFiles(options.ProjectDirectory, "*.sln", SearchOption.TopDirectoryOnly)
                                             .Concat(Directory.GetFiles(options.ProjectDirectory, "*.slnx", SearchOption.AllDirectories)))
        {
            File.Delete(solutionFile);
            Console.WriteLine($"Deleted extra solution file: {solutionFile}");
        }

        // Link the project to the solution
        await cliService.RunAsync($"dotnet sln add {options.ProjectDirectory}/{options.ProjectName}/{options.ProjectName}.csproj", options.TargetDirectory);
    }

    public async Task AddProjectReferenceAsync(string workingDirectory, string referencingCsprojPath, string referencedCsprojPath, CancellationToken ct = default)
    {
        if (File.Exists(referencingCsprojPath) && File.Exists(referencedCsprojPath))
        {
            await cliService.RunAsync($"dotnet add {referencingCsprojPath} reference {referencedCsprojPath}", workingDirectory, ct);
            Console.WriteLine($"Project Linked: {referencingCsprojPath} → {referencedCsprojPath}");
        }
    }
}
