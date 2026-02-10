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
        await cliService.RunAsync($"dotnet new {options.Template} -n {options.ProjectName}", options.ProjectDirectory);

        // Link the project to the solution
        await cliService.RunAsync($"dotnet sln add {options.ProjectDirectory}/{options.ProjectName}/{options.ProjectName}.csproj", options.TargetDirectory);

        // Remove the default Class1.cs file if it exists
        var defaultClassFilePath = Path.Combine(options.ProjectDirectory, options.ProjectName, "Class1.cs");
        if (File.Exists(defaultClassFilePath))
            File.Delete(defaultClassFilePath);

    }
}
