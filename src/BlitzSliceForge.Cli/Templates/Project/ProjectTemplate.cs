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
            GetDomainTestsProject(generationOptions),
            GetApplicationTestsProject(generationOptions),
            GetInfrastructureTestsProject(generationOptions)
        };
    }

    private static ProjectGenerationOptions GetDomainProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Domain", "classlib", generationOptions.OutputDirectory!, false, null);
    }

    private static ProjectGenerationOptions GetApplicationProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Application", "classlib", generationOptions.OutputDirectory!, false, null);
    }

    private static ProjectGenerationOptions GetInfrastructureProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Infrastructure", "classlib", generationOptions.OutputDirectory!, false, null);
    }

    private static ProjectGenerationOptions GetBlazorProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Web", "blazor", generationOptions.OutputDirectory!, false, "--interactivity Auto");
    }

    private static ProjectGenerationOptions GetDomainTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Domain", "classlib", generationOptions.OutputDirectory!, true, null);
    }

    private static ProjectGenerationOptions GetApplicationTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Application", "classlib", generationOptions.OutputDirectory!, true, null);
    }

    private static ProjectGenerationOptions GetInfrastructureTestsProject(GenerationOptions generationOptions)
    {
        return new ProjectGenerationOptions(generationOptions.SolutionName, "Infrastructure", "classlib", generationOptions.OutputDirectory!, true, null);
    }
}
