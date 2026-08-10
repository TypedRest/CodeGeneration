using CommandLine;
using Microsoft.CodeAnalysis.CSharp;
using TypedRest.CodeGeneration.CSharp;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.Cli.Commands;

[Verb("generate", HelpText = "Generates a TypedRest client.")]
public class Generate : CommandBase
{
    [Option('o', "output", HelpText = "The directory to write the generated source code to.", Required = true)]
    public string OutputDir { get; set; } = default!;

    [Option('s', "service-name", HelpText = "The service name to use for the entry endpoint.", Required = true)]
    public string ServiceName { get; set; } = default!;

    [Option('n', "namespace", HelpText = "The C# namespace for the endpoint. Uses service-name if not set.")]
    public string? Namespace { get; set; }

    [Option("dto-namespace", HelpText = "The C# namespace for the DTOs. Uses namespace if not set.")]
    public string? DtoNamespace { get; set; }

    [Option("generate-interfaces", HelpText = "Controls whether to generate interfaces for endpoints.")]
    public bool GenerateInterfaces { get; set; }

    [Option("generate-dtos", HelpText = "Controls whether to generate DTOs.")]
    public bool GenerateDtos { get; set; }

    [Option("generate-entry-constructor", Default = true, HelpText = "Controls whether the entry endpoint gets a constructor taking the base URI. Turn this off to supply the constructors yourself in a partial class.")]
    public bool GenerateEntryConstructor { get; set; } = true;

    [Option("lang-version", Default = "latest", HelpText = "The minimum C# version the generated DTOs must compile with, using the same values as the MSBuild LangVersion property.")]
    public string LangVersion { get; set; } = "latest";

    public override int Run()
    {
        if (!LanguageVersionFacts.TryParse(LangVersion, out var languageVersion))
        {
            Console.Error.WriteLine($"Error: Invalid --lang-version '{LangVersion}'. Expected a value such as '8', '11.0' or 'latest'.");
            return 1;
        }

        IClientGenerator generator = new CSharpClientGenerator();
        var options = new GenerationOptions(ServiceName)
        {
            Namespace = Namespace,
            DtoNamespace = DtoNamespace,
            GenerateInterfaces = GenerateInterfaces,
            GenerateDtos = GenerateDtos,
            GenerateEntryConstructor = GenerateEntryConstructor,
            LanguageVersion = languageVersion
        };

        var (doc, _) = ReadDoc();

        Directory.CreateDirectory(OutputDir);
        foreach (var file in generator.Generate(doc, options, new ConsoleGenerationLog()))
            file.WriteToDirectory(OutputDir);

        return 0;
    }
}
