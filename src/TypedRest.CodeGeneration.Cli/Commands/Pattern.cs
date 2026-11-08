using System.Text;

namespace TypedRest.CodeGeneration.Cli.Commands;

[Verb("pattern", HelpText = "Finds TypedRest endpoint patterns.")]
public class Pattern : CommandBase
{
    /// <summary>
    /// File formats for OpenAPI documents.
    /// </summary>
    public enum Format
    {
        Yaml,
        Json
    }

    [Option('o', "output", HelpText = "The path to the Swagger or OpenAPI spec file to write. Uses input file name if not set.")]
    public string? OutputPath { get; set; }

    [Option("output-version", HelpText = "The output version. Valid values are 'OpenApi2_0' (Swagger), 'OpenApi3_0', 'OpenApi3_1' and 'OpenApi3_2'. Uses input version if not set.")]
    public OpenApiSpecVersion? OutputVersion { get; set; }

    [Option("output-format", HelpText = "The output format. Valid values are 'Yaml' and 'Json'. Determines format based on file ending if not set.")]
    public Format? OutputFormat { get; set; }

    public override int Run()
    {
        var (doc, version) = ReadDoc();

        doc.SetTypedRest(doc.MatchTypedRestPatterns());

        var outputVersion = OutputVersion ?? version;
        var serialized = (OutputFormat ?? GuessFormat()) == Format.Json
            ? doc.SerializeAsJsonAsync(outputVersion)
            : doc.SerializeAsYamlAsync(outputVersion);
        Write(serialized.GetAwaiter().GetResult());

        return 0;
    }

    private Format GuessFormat()
        => (OutputPath ?? InputPath).EndsWith(".json")
            ? Format.Json
            : Format.Yaml;

    private void Write(string data)
    {
        if (OutputPath == "-")
            Console.Write(data);
        else
            File.WriteAllText(OutputPath ?? InputPath, data, Encoding.UTF8);
    }
}
