using CommandLine;
using TypedRest.CodeGeneration.CSharp;
using NanoByte.CodeGeneration;

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

    public override int Run()
    {
        var (doc, _) = ReadDoc();
        var naming = new NamingStrategy(ServiceName, Namespace ?? ServiceName, DtoNamespace ?? Namespace ?? ServiceName);

        var types = doc.GenerateTypedRestEndpoints(naming, GenerateInterfaces);
        if (GenerateDtos)
            types = types.Concat(doc.GenerateDtos(naming));

        WriteSource(types);

        return 0;
    }

    private void WriteSource(IEnumerable<ICSharpType> types)
    {
        Directory.CreateDirectory(OutputDir);
        foreach (var type in types)
            type.WriteToDirectory(OutputDir);
    }
}
