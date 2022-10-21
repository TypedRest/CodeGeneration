using System.Text;

namespace TypedRest.CodeGeneration.Cli.Commands;

[Verb("pattern", HelpText = "Finds TypedRest endpoint patterns.")]
public class Pattern : CommandBase
{
    [Option('o', "output", HelpText = "The path to the Swagger or OpenAPI spec file to write. Uses input file name if not set.")]
    public string? OutputPath { get; set; }

    [Option("output-version", HelpText = "The output version. Valid values are 'OpenApi2_0' (Swagger) and 'OpenApi3_0'. Uses input version if not set.")]
    public OpenApiSpecVersion? OutputVersion { get; set; }

    [Option("output-format", HelpText = "The output format. Valid values are 'Yaml' and 'Json'. Determines format based on file ending if not set.")]
    public OpenApiFormat? OutputFormat { get; set; }

    public override int Run()
    {
        var (doc, version) = ReadDoc();

        doc.SetTypedRest(doc.MatchTypedRestPatterns());

        Write(doc.Serialize(
            OutputVersion ?? version,
            OutputFormat ?? GuessFormat()));

        return 0;
    }

    private OpenApiFormat GuessFormat()
        => (OutputPath ?? InputPath).EndsWith(".json")
            ? OpenApiFormat.Json
            : OpenApiFormat.Yaml;

    private void Write(string data)
    {
        if (OutputPath == "-")
            Console.Write(data);
        else
            File.WriteAllText(OutputPath ?? InputPath, data, Encoding.UTF8);
    }
}
