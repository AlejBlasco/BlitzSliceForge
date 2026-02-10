namespace BlitzSliceForge.Cli.Models;

public class ProjectGenerationOptions
{
    public string SolutionName { get; set; }
    public string Suffix { get; set; }
    public string Template { get; set; }
    public string? Options { get; set; }
    public bool IsTestProject { get; set; } = false;
    public string TargetDirectory => targetDirectory;

    private readonly string targetDirectory;

    public ProjectGenerationOptions(string solutionName, string suffix, string template, string targetDirectory, bool isTestProject = false, string? options = null)
    {
        SolutionName = solutionName;
        Suffix = suffix;
        Template = template;
        IsTestProject = isTestProject;
        Options = options;
        this.targetDirectory = targetDirectory;
    }

    public string ProjectName => $"{SolutionName}.{Suffix}";
    
    public string ProjectDirectory => IsTestProject 
        ? Path.Combine(targetDirectory, "tests") 
        : Path.Combine(targetDirectory, "src");
}
