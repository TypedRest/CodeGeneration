namespace TypedRest.CodeGeneration.Cli.Commands;

public abstract class CommandBase
{
    public abstract int Run();

    [Option('f', "file", HelpText = "The path to the Swagger or OpenAPI spec file.", Required = true)]
    public string InputPath { get; set; } = default!;

    protected (OpenApiDocument, OpenApiSpecVersion) ReadDoc()
    {
        using var stream = InputPath == "-"
            ? Console.OpenStandardInput()
            : File.OpenRead(InputPath);
        var (doc, diagnostic) = OpenApiDocumentReader.Read(stream);

        foreach (var warning in diagnostic?.Warnings ?? [])
            Console.Error.WriteLine("Warning: " + warning.Message);
        foreach (var error in diagnostic?.Errors ?? [])
            Console.Error.WriteLine("Error: " + error.Message);

        if (doc == null) throw new InvalidDataException($"Unable to parse {InputPath} as an OpenAPI or Swagger document.");
        return (doc, diagnostic!.SpecificationVersion);
    }
}
