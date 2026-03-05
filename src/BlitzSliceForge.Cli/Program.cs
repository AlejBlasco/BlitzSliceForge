using BlitzSliceForge.Cli.Enums;
using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Services;
using System.CommandLine;

namespace BlitzSliceForge.Cli;

static class Program
{
    static async Task Main(string[] args)
    {
        RootCommand rootCommand = new("bsf - Blitz Slice Forge");

        Option<string> nameOption = new("--name", "-n")
        {
            Description = "Name of the solution",
            Arity = ArgumentArity.ExactlyOne,
            Required = true
        };
        Option<AvailableFrameworksEnum> frameworkOption = new("--framework", "-f")
        {
            Description = "Target .NET framework version (e.g. net8)",
            Arity = ArgumentArity.ZeroOrOne
        };

        Option<DirectoryInfo?> outputOption = new("--output", "-o")
        {
            Description = "Output path (default: solution name)",
            Arity = ArgumentArity.ZeroOrOne
        };

        Option<string> featuresOption = new("--features", "-ft")
        {
            Description = "Comma-separated list of features to include",
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.ZeroOrOne
        };

        rootCommand.Options.Add(nameOption);
        rootCommand.Options.Add(frameworkOption);
        rootCommand.Options.Add(outputOption);
        rootCommand.Options.Add(featuresOption);

        rootCommand.SetAction(async (ParseResult parseResult, CancellationToken ct) =>
        {

            //TODO: Add validation for output path and solution name (e.g. invalid chars, reserved names, etc.)
            //TODO: Add validation for features (e.g. check if the provided features are not empty, contain invalid chars, etc.)

            string? name = parseResult.GetValue(nameOption);
            AvailableFrameworksEnum framework = parseResult.GetValue(frameworkOption);
            string? output = parseResult.GetValue(outputOption)?.FullName ?? name;
            string[]? features = parseResult.GetValue(featuresOption)?.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(output))
            {
                var cliService = new Services.DotNetCliService();
                var templateRendererService = new Services.TemplateRendererService();
                var solutionGenerator = new Generators.SolutionGenerator(cliService, templateRendererService);

                var options = new GenerationOptions
                {
                    SolutionName = name!,
                    OutputDirectory = output!,
                    Framework = framework.ToFrameworkString(),
                    Features = features
                };

                SdkCheckerService sdkChecker = new(cliService);
                if (await sdkChecker.ValidateFrameworkAsync(options.Framework, ct))
                {
                    await solutionGenerator.GenerateAsync(options, ct);
                }
            }
        });

        await rootCommand.Parse(args).InvokeAsync();
    }
}
