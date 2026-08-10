using CommandLine;
using Microsoft.CodeAnalysis.CSharp;
using TypedRest.CodeGeneration.CSharp;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript;

namespace TypedRest.CodeGeneration.Cli.Commands;

[Verb("generate", HelpText = "Generates a TypedRest client.")]
public class Generate : CommandBase
{
    [Option('o', "output", HelpText = "The directory to write the generated source code to.", Required = true)]
    public string OutputDir { get; set; } = default!;

    [Option('s', "service-name", HelpText = "The service name to use for the entry endpoint.", Required = true)]
    public string ServiceName { get; set; } = default!;

    [Option('l', "language", Default = CSharpClientGenerator.LanguageName, HelpText = "The language to generate: 'csharp' or 'typescript'.")]
    public string Language { get; set; } = CSharpClientGenerator.LanguageName;

    [Option('n', "namespace", HelpText = "The C# namespace for the endpoints, or the directory for TypeScript. Uses service-name if not set.")]
    public string? Namespace { get; set; }

    [Option("dto-namespace", HelpText = "The C# namespace for the DTOs, or the directory for TypeScript. Uses namespace if not set.")]
    public string? DtoNamespace { get; set; }

    [Option("generate-interfaces", HelpText = "Controls whether to generate interfaces for endpoints. C# only.")]
    public bool GenerateInterfaces { get; set; }

    [Option("generate-dtos", HelpText = "Controls whether to generate DTOs.")]
    public bool GenerateDtos { get; set; }

    [Option("generate-entry-constructor", Default = true, HelpText = "Controls whether the entry endpoint gets a constructor taking the base URI. Turn this off to supply the constructors yourself in a partial class. C# only.")]
    public bool GenerateEntryConstructor { get; set; } = true;

    [Option("lang-version", Default = "latest", HelpText = "The minimum C# version the generated DTOs must compile with, using the same values as the MSBuild LangVersion property. C# only.")]
    public string LangVersion { get; set; } = "latest";

    [Option("serializer", HelpText = "The JSON serializer the generated DTOs are annotated for. C#: 'newtonsoft' (default) or 'system-text-json'. Ignored for languages that do not annotate DTOs.")]
    public string? Serializer { get; set; }

    /// <summary>
    /// The target languages this tool can generate.
    /// </summary>
    private static ClientGeneratorRegistry Generators
        => new ClientGeneratorRegistry()
          .Add(new CSharpClientGenerator())
          .Add(new TypeScriptClientGenerator());

    public override int Run()
    {
        var generators = Generators;
        if (!generators.TryGet(Language, out var generator))
        {
            Console.Error.WriteLine($"Error: Unknown language '{Language}'. Expected one of: {string.Join(", ", generators.Languages)}.");
            return 1;
        }

        var options = generator.CreateOptions(ServiceName);
        options.Namespace = Namespace;
        options.DtoNamespace = DtoNamespace;
        options.GenerateInterfaces = GenerateInterfaces;
        options.GenerateDtos = GenerateDtos;
        options.GenerateEntryConstructor = GenerateEntryConstructor;

        var log = new ConsoleGenerationLog();

        if (Serializer != null)
        {
            if (options.SupportedSerializers.Count == 0)
                log.Report(Messages.SerializerNotSupported());
            else if (!options.SupportedSerializers.Contains(Serializer, StringComparer.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine($"Error: Unknown serializer '{Serializer}' for language '{generator.Language}'. Expected one of: {string.Join(", ", options.SupportedSerializers)}.");
                return 1;
            }
            else options.Serializer = Serializer;
        }

        if (options is GenerationOptions csharp)
        {
            if (!LanguageVersionFacts.TryParse(LangVersion, out var languageVersion))
            {
                Console.Error.WriteLine($"Error: Invalid --lang-version '{LangVersion}'. Expected a value such as '8', '11.0' or 'latest'.");
                return 1;
            }
            csharp.LanguageVersion = languageVersion;
        }
        else if (LangVersion != "latest")
            log.Report(Messages.LangVersionNotSupported());

        var (doc, _) = ReadDoc();

        Directory.CreateDirectory(OutputDir);
        foreach (var file in generator.Generate(doc, options, log))
            file.WriteToDirectory(OutputDir);

        return 0;
    }
}
