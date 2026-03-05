using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlitzSliceForge.Cli.Models;
using BlitzSliceForge.Cli.Services;
using BlitzSliceForge.Cli.Templates.Project;

namespace BlitzSliceForge.Cli.Generators;

public class FeatureGenerator
{
    private readonly TemplateRendererService templateRendererService;

    public FeatureGenerator()
    {
        templateRendererService = new TemplateRendererService();
    }

    public async Task AddFeatureAsync(string featureName, GenerationOptions generationOptions)
    {
        Console.WriteLine($"Generating feature '{featureName}'...");

        var normalizedFeatureName = featureName.Trim();

        ProjectGenerationOptions? applicationOptions = ProjectTemplate.GetAvailableProjects(generationOptions)
            .FirstOrDefault(p => p.Suffix == "Application");

        ProjectGenerationOptions? sharedOptions = ProjectTemplate.GetAvailableProjects(generationOptions)
            .FirstOrDefault(p => p.Suffix == "Shared");

        if (applicationOptions == null || sharedOptions == null)
            return;

        // Ensure the shared project has a Features folder for shared DTOs, etc.
        string sharedFeaturesDir = Path.Combine(sharedOptions.ProjectDirectory, sharedOptions.ProjectName, normalizedFeatureName);
        if (!Directory.Exists(sharedFeaturesDir))
            Directory.CreateDirectory(sharedFeaturesDir);

        await templateRendererService.RenderAndSaveAsync(
            templatePath: "Templates/Features/Dto.cs.tmpl",
            outputPath: Path.Combine(sharedFeaturesDir, $"{normalizedFeatureName}Dto.cs"),
            model: new
            {
                SolutionName = generationOptions.SolutionName,
                FeatureName = normalizedFeatureName
            }
        );

        // Create the feature directory if it doesn't exist
        string appFeaturesDir = Path.Combine(applicationOptions.ProjectDirectory, applicationOptions.ProjectName, "Features");
        if (!Directory.Exists(appFeaturesDir))
            Directory.CreateDirectory(appFeaturesDir);

        appFeaturesDir = Path.Combine(appFeaturesDir, normalizedFeatureName);

        // Create the query
        await templateRendererService.RenderAndSaveAsync(
            templatePath: "Templates/Features/GetByIdQuery.cs.tmpl",
            outputPath: Path.Combine(appFeaturesDir, $"Get{normalizedFeatureName}ById.cs"),
            model: new
                {
                    SolutionName = generationOptions.SolutionName,
                    FeatureName = normalizedFeatureName
                }
        );

        // Create the commands
        await templateRendererService.RenderAndSaveAsync(
            templatePath: "Templates/Features/CreateCommand.cs.tmpl",
            outputPath: Path.Combine(appFeaturesDir, $"Create{normalizedFeatureName}.cs"),
            model: new
            {
                SolutionName = generationOptions.SolutionName,
                FeatureName = normalizedFeatureName
            }
        );

        await templateRendererService.RenderAndSaveAsync(
            templatePath: "Templates/Features/EditCommand.cs.tmpl",
            outputPath: Path.Combine(appFeaturesDir, $"Edit{normalizedFeatureName}.cs"),
            model: new
            {
                SolutionName = generationOptions.SolutionName,
                FeatureName = normalizedFeatureName
            }
        );

        await templateRendererService.RenderAndSaveAsync(
            templatePath: "Templates/Features/DeleteCommand.cs.tmpl",
            outputPath: Path.Combine(appFeaturesDir, $"Delete{normalizedFeatureName}.cs"),
            model: new
            {
                SolutionName = generationOptions.SolutionName,
                FeatureName = normalizedFeatureName
            }
        );

        Console.WriteLine($"Feature '{normalizedFeatureName} successfully generated.'");
    }
}
