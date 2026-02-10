using Scriban;

namespace BlitzSliceForge.Cli.Services
{
    public class TemplateRendererService
    {
        public async Task RenderAndSaveAsync(string templatePath, string outputPath, object? model = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (!File.Exists(templatePath))
            {
                Console.WriteLine($"Template file not found: {templatePath}");
                return;
            }

            var templateContent = await File.ReadAllTextAsync(templatePath, ct);
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

            ct.ThrowIfCancellationRequested();

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await File.WriteAllTextAsync(outputPath, rendered, ct);
        }
    }
}
