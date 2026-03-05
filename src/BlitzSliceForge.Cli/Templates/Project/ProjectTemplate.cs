using BlitzSliceForge.Cli.Models;

namespace BlitzSliceForge.Cli.Templates.Project;

public static class ProjectTemplate
{
    public static List<ProjectGenerationOptions> GetAvailableProjects(GenerationOptions generationOptions)
    {
        return new List<ProjectGenerationOptions>
        {
            GetDomainProject(generationOptions),
            GetApplicationProject(generationOptions),
            GetInfrastructureProject(generationOptions),
            GetBlazorProject(generationOptions),
            GetSharedProject(generationOptions),
            GetDomainTestsProject(generationOptions),
            GetApplicationTestsProject(generationOptions),
            GetInfrastructureTestsProject(generationOptions)
        };
    }

    private static ProjectGenerationOptions GetDomainProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Domain", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, false, null);
    }

    private static ProjectGenerationOptions GetApplicationProject(GenerationOptions generationOptions)
    {
        var projectOptions = new ProjectGenerationOptions(generationOptions.SolutionName, "Application", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, false, null);
        projectOptions.Packages = new[] { "MediatR" };

        return projectOptions;
    }

    private static ProjectGenerationOptions GetInfrastructureProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Infrastructure", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, false, null);
    }

    private static ProjectGenerationOptions GetBlazorProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Web", "blazor", generationOptions.OutputDirectory!, generationOptions.Framework, false, "--interactivity Auto");
    }

    private static ProjectGenerationOptions GetSharedProject(GenerationOptions generationOptions)
    {
        var projectOptions = new ProjectGenerationOptions(generationOptions.SolutionName, "Shared", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, false, null);
        projectOptions.Packages = new[] { "FluentValidation" };
        return projectOptions;  
    }

    private static ProjectGenerationOptions GetDomainTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Domain", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, true, null);
    }

    private static ProjectGenerationOptions GetApplicationTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Application", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, true, null);
    }

    private static ProjectGenerationOptions GetInfrastructureTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Infrastructure", "classlib", generationOptions.OutputDirectory!, generationOptions.Framework, true, null);
    }
}
