using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Services;
using BlitzSliceForge.Cli.Templates.Project;

namespace BlitzSliceForge.Cli.Generators;

public class SolutionGenerator
{
    private readonly DotNetCliService cliService;
    private readonly TemplateRendererService templateRenderer;

    private readonly ProjectGenerator projectGenerator;

    public SolutionGenerator(DotNetCliService cliService, TemplateRendererService templateRenderer)
    {
        this.cliService = cliService
            ?? throw new ArgumentNullException(nameof(cliService));

        this.templateRenderer = templateRenderer
            ?? throw new ArgumentNullException(nameof(templateRenderer));

        this.projectGenerator = new ProjectGenerator(cliService);
    }

    public async Task GenerateAsync(GenerationOptions options, CancellationToken ct = default)
    {
        Console.WriteLine($"Generating solution: {options.SolutionName} in {options.OutputDirectory} ...");

        if (!Directory.Exists(options.OutputDirectory!))
            Directory.CreateDirectory(options.OutputDirectory!);

        // Generate the solution file
        await cliService.RunAsync($"dotnet new sln -n {options.SolutionName}", options.OutputDirectory!);

        // Generate principal folders
        GeneratePrincipalFolders(options.OutputDirectory!);

        // Common templates rendering (e.g. .editorconfig, .gitignore, README.md, etc.)
        await GenerateCommonTemplates(options, ct);

        // Generate projects
        await GenerateProjetcs(options, ct);
    }

    private void GeneratePrincipalFolders(string targetDir)
    {
        Console.WriteLine("Generating principal folders ...");

        var folders = new[] { "src", "tests", "docs/adr", ".github/workflows", "docker" };
        foreach (var folder in folders)
        {
            var folderPath = Path.Combine(targetDir, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
    }

    private async Task GenerateCommonTemplates(GenerationOptions options, CancellationToken ct = default)
    {
        var commonTemplatesPath = Path.Combine(AppContext.BaseDirectory, "Templates", "Common");

        await templateRenderer.RenderAndSaveAsync(Path.Combine(commonTemplatesPath, "global.json"),
            Path.Combine(options.OutputDirectory!, "global.json"), null, ct);

        await templateRenderer.RenderAndSaveAsync(Path.Combine(commonTemplatesPath, "Directory.Build.props"),
            Path.Combine(options.OutputDirectory!, "Directory.Build.props"), null, ct);

        await templateRenderer.RenderAndSaveAsync(Path.Combine(commonTemplatesPath, ".gitignore"),
            Path.Combine(options.OutputDirectory!, ".gitignore"), null, ct);

        await templateRenderer.RenderAndSaveAsync(Path.Combine(commonTemplatesPath, "README-Template.md"),
            Path.Combine(options.OutputDirectory!, "README.md"),
             new Dictionary<string, object>
            {
                { "SolutionName", options.SolutionName }
            },
            ct);
    }

    private async Task GenerateProjetcs(GenerationOptions options, CancellationToken ct = default)
    {
        // Minimun projects
        var projects = ProjectTemplate.GetAvailableProjects(options);
        foreach (var project in projects)
        {
            await projectGenerator.CreateProjectAsync(project, ct);
        }

        // Linking projects to others   
        var applicationProject = projects.FirstOrDefault(p => p.Suffix == "Application");
        var domainProject = projects.FirstOrDefault(p => p.Suffix == "Domain");
        var infrastructureProject = projects.FirstOrDefault(p => p.Suffix == "Infrastructure");

        if (applicationProject != null && domainProject != null)
            await projectGenerator.AddProjectReferenceAsync(options.OutputDirectory!, applicationProject.FullProjectPath, domainProject.FullProjectPath, ct);
        if (applicationProject != null && infrastructureProject != null)
            await projectGenerator.AddProjectReferenceAsync(options.OutputDirectory!, applicationProject.FullProjectPath, infrastructureProject.FullProjectPath, ct);

        if (infrastructureProject != null && domainProject != null)
            await projectGenerator.AddProjectReferenceAsync(options.OutputDirectory!, infrastructureProject.FullProjectPath, domainProject.FullProjectPath, ct);
    }
}
