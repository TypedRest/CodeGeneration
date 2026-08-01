using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Runs <see cref="TypedRestSourceGenerator"/> the same way the compiler does.
/// </summary>
/// <remarks>
/// The analyzer config keys built here are the contract that <c>build/*.props</c> (which declares them) and
/// <c>build/*.targets</c> (which populates them) have to honour.
/// </remarks>
public class GeneratorTestHarness(string yaml, string path = "sample-v3.yml")
{
    private readonly Dictionary<string, string> _metadata = [];
    private readonly Dictionary<string, string> _properties = [];

    /// <summary>Sets <c>TypedRestOpenApi</c> item metadata.</summary>
    public GeneratorTestHarness WithMetadata(string name, string value)
    {
        _metadata["build_metadata.TypedRestOpenApi." + name] = value;
        return this;
    }

    /// <summary>Sets an MSBuild property. <paramref name="name"/> is the full property name.</summary>
    public GeneratorTestHarness WithProperty(string name, string value)
    {
        _properties["build_property." + name] = value;
        return this;
    }

    /// <summary>Marks the file as a TypedRest spec and sets the service name.</summary>
    public GeneratorTestHarness ForService(string serviceName)
        => AsSpec().WithMetadata("ServiceName", serviceName);

    /// <summary>Marks the file as a TypedRest spec, the way the <c>TypedRestOpenApi</c> item definition does.</summary>
    public GeneratorTestHarness AsSpec()
        => WithMetadata("IsTypedRestSpec", "true");

    /// <summary>Runs the generator, optionally <paramref name="twice"/> to check that the second run hits the cache.</summary>
    public GeneratorRunResult Run(bool twice = false)
    {
        var optionsProvider = new TestOptionsProvider(new TestOptions(_properties), new TestOptions(_metadata));

        var compilation = CSharpCompilation.Create("TestAssembly",
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new TypedRestSourceGenerator().AsSourceGenerator()],
            additionalTexts: [new TestAdditionalText(path, yaml)],
            parseOptions: new CSharpParseOptions(LanguageVersion.CSharp12),
            optionsProvider: optionsProvider,
            driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

        driver = driver.RunGenerators(compilation);
        if (twice) driver = driver.RunGenerators(compilation);

        return driver.GetRunResult().Results.Single();
    }

    private sealed class TestAdditionalText(string path, string text) : AdditionalText
    {
        public override string Path { get; } = path;

        public override SourceText GetText(CancellationToken cancellationToken = default)
            => SourceText.From(text, Encoding.UTF8);
    }

    private sealed class TestOptions(Dictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
            => values.TryGetValue(key, out value!);
    }

    private sealed class TestOptionsProvider(AnalyzerConfigOptions global, AnalyzerConfigOptions perFile) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = global;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => perFile;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => perFile;
    }
}
