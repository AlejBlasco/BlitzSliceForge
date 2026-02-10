using BlitzSliceForge.Cli.Models;
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

        rootCommand.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {
            string? name = parseResult.GetValue(nameOption);
            string? output = parseResult.GetValue(outputOption)?.FullName ?? name;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(output))
            {
                var cliService = new Services.DotNetCliService();
                var templateRendererService = new Services.TemplateRendererService();
                var solutionGenerator = new Generators.SolutionGenerator(cliService, templateRendererService);

                var options = new GenerationOptions
                {
                    SolutionName = name!,
                    OutputDirectory = output!
                };

                await solutionGenerator.GenerateAsync(options, ct);
            }
        });

        await rootCommand.Parse(args).InvokeAsync();
    }
}
