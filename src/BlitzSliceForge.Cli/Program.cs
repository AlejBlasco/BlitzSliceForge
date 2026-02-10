using Scriban;
using System.CommandLine;

namespace BlitzSliceForge.Cli;

class Program
{
    static async Task Main(string[] args)
    {

        RootCommand rootCommand = new("bsf - Blitz Slice Forge");

        Option<string> nameOption = new("--name", "-n")
        {
            Description = "Name of the solution",
            Required = true
        };
        Option<DirectoryInfo?> outputOption = new("--output", "-o")
        {
            Description = "Output path (default: solution name)",
            Arity = ArgumentArity.ZeroOrOne
        };

        rootCommand.Options.Add(nameOption);
        rootCommand.Options.Add(outputOption);

        rootCommand.SetAction(async parseResult =>
        {
            string? name = parseResult.GetValue(nameOption);
            string? output = parseResult.GetValue(outputOption)?.FullName ?? name;
            
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(output))
                await GenerateSolution(name, output);
        });

        await rootCommand.Parse(args).InvokeAsync();
    }

    private static async Task GenerateSolution(string solutionName, string targetDir, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Generating solution: {solutionName} in {targetDir} ...");
        Directory.CreateDirectory(targetDir);

        // Generate the solution file
        Run("dotnet new sln -n " + solutionName, targetDir);

        // Generate principal folders
        var folders = new[] { "src", "tests", "docs/adr", ".github/workflows", "docker" };
        foreach (var folder in folders)
        {
            var folderPath = Path.Combine(targetDir, folder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        // Minimun projects
        var projects = new[]
        {
            ("Domain", "classlib", ""),
            ("Application", "classlib", ""),
            ("Infrastructure", "classlib", ""),
            //("Web", "blazor", "--interactivity Auto") //TODO: Revisar, no se linkea bien
		};

        foreach (var (suffix, template, options) in projects)
        {
            var projectName = $"{solutionName}.{suffix}";
            var projectDir = Path.Combine(targetDir, "src");
            if (!Directory.Exists(projectDir))
                Directory.CreateDirectory(projectDir);

            Console.WriteLine($"Generating and linking project: {projectName} in {projectDir} ...");

            // Generate project
            if (!string.IsNullOrWhiteSpace(options))
                Run($"dotnet new {template} -n {projectName} {options}", projectDir);
            else
                Run($"dotnet new {template} -n {projectName}", projectDir);

            // Link project to solution
            Run($"dotnet sln add {projectDir}/{projectName}/{projectName}.csproj", targetDir);

            var defaultClassFilePath = Path.Combine(projectDir, projectName, "Class1.cs");
            if (File.Exists(defaultClassFilePath))
                File.Delete(defaultClassFilePath);
        }

        // Copy / Render common files
        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates");

        await RenderTemplate(
            Path.Combine(templatesPath, "Common", "global.json"),
            Path.Combine(targetDir, "global.json"),
            new Dictionary<string, object>
            {
                { "SolutionName", solutionName }
            });

        await RenderTemplate(
            Path.Combine(templatesPath, "Common", "Directory.Build.props"),
            Path.Combine(targetDir, "Directory.Build.props"),
            new Dictionary<string, object>
            {
                { "SolutionName", solutionName }
            });

        await RenderTemplate(
            Path.Combine(templatesPath, "Common", "README-Template.md"),
            Path.Combine(targetDir, "README.md"),
            new Dictionary<string, object>
            {
                { "SolutionName", solutionName }
            });

        Console.WriteLine("\nSolution created.");
        Console.WriteLine($"Next steps:");
        Console.WriteLine($"  cd {solutionName}");
        Console.WriteLine($"  code .");
        Console.WriteLine($"  cd src/{solutionName}.Web && dotnet run");
    }

    private static void Run(string cmd, string workingDir)
    {
        var psi = new System.Diagnostics.ProcessStartInfo("dotnet", cmd)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            //CreateNoWindow = true
        };

        using var process = System.Diagnostics.Process.Start(psi)!;
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"Error executing: dotnet {cmd}\n{process.StandardError.ReadToEnd()}");
    }

    private static async Task RenderTemplate(string templatePath, string outputPath, object model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Template file not found: {templatePath}");
            return;
        }

        var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
        var template = Template.Parse(templateContent);

        if (template == null || template.HasErrors)
        {
            Console.WriteLine($"Template has error: {templatePath}");
            foreach (var error in template!.Messages)
            {
                Console.WriteLine($"Error: {error}");
            }
            return;
        }

        var rendered = await template.RenderAsync(model);

        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, rendered, cancellationToken);
    }
}
