using Microsoft.CodeAnalysis;

namespace TypedRest.CodeGeneration.CSharp;

public class TypedRestSourceGeneratorFacts
{
    [Fact]
    public void GeneratesEndpointsAndDtos()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        HintNames(result).Should().BeEquivalentTo([
            "sample-v3.MyNamespace.IMyServiceClient.g.cs",
            "sample-v3.MyNamespace.MyServiceClient.g.cs",
            "sample-v3.MyNamespace.IContactElementEndpoint.g.cs",
            "sample-v3.MyNamespace.ContactElementEndpoint.g.cs",
            "sample-v3.MyNamespace.Contact.g.cs",
            "sample-v3.MyNamespace.Note.g.cs"
        ]);
        Source(result, "sample-v3.MyNamespace.MyServiceClient.g.cs")
           .Should().Contain("namespace MyNamespace")
           .And.Contain("public partial class MyServiceClient : EntryEndpoint, IMyServiceClient");
    }

    [Fact]
    public void GeneratesFromSwagger2()
    {
        var result = new GeneratorTestHarness(Sample.YamlV2, path: "sample-v2.yml")
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        HintNames(result).Should().Contain([
            "sample-v2.MyNamespace.MyServiceClient.g.cs",
            "sample-v2.MyNamespace.Contact.g.cs"
        ]);
    }

    [Fact]
    public void SkipsUnmarkedAdditionalFiles()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .WithMetadata("ServiceName", "MyService")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }

    [Fact]
    public void ReportsMissingServiceName()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .AsSpec()
                    .Run();

        result.Diagnostics.Select(x => x.Id).Should().BeEquivalentTo(["TRCG001"]);
        result.GeneratedSources.Should().BeEmpty();
    }

    [Fact]
    public void ReportsSpecErrors()
    {
        var result = new GeneratorTestHarness("openapi: 3.0.0")
                    .ForService("MyService")
                    .Run();

        result.Diagnostics.Select(x => x.Id).Should().Contain("TRCG002");
    }

    [Fact]
    public void ReportsUnparsableSpec()
    {
        var result = new GeneratorTestHarness("this is not an OpenAPI document")
                    .ForService("MyService")
                    .Run();

        result.Diagnostics.Select(x => x.Id).Should().BeEquivalentTo(["TRCG005"]);
        result.GeneratedSources.Should().BeEmpty();
    }

    [Fact]
    public void WarnsOnInvalidLangVersion()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("LangVersion", "bogus")
                    .Run();

        result.Diagnostics.Select(x => x.Id).Should().BeEquivalentTo(["TRCG004"]);
        result.GeneratedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void UsesRootNamespaceAsFallback()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithProperty("RootNamespace", "Fallback.Namespace")
                    .Run();

        HintNames(result).Should().Contain("sample-v3.Fallback.Namespace.MyServiceClient.g.cs");
    }

    [Fact]
    public void UsesPropertiesAsFallback()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .AsSpec()
                    .WithProperty("TypedRestServiceName", "MyService")
                    .WithProperty("TypedRestNamespace", "MyNamespace")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        HintNames(result).Should().Contain("sample-v3.MyNamespace.MyServiceClient.g.cs");
    }

    [Fact]
    public void DefaultsToGeneratingInterfacesAndDtos()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3).ForService("MyService").Run();

        HintNames(result).Should().Contain([
            "sample-v3.MyService.IMyServiceClient.g.cs",
            "sample-v3.MyService.Contact.g.cs"
        ]);
    }

    [Fact]
    public void HonorsGenerateInterfacesAndDtosBeingDisabled()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("GenerateInterfaces", "false")
                    .WithMetadata("GenerateDtos", "false")
                    .Run();

        HintNames(result).Should().BeEquivalentTo([
            "sample-v3.MyService.MyServiceClient.g.cs",
            "sample-v3.MyService.ContactElementEndpoint.g.cs"
        ]);
    }

    [Fact]
    public void HonorsGenerateEntryConstructorBeingDisabled()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .WithMetadata("GenerateEntryConstructor", "false")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        Source(result, "sample-v3.MyNamespace.MyServiceClient.g.cs")
           .Should().Contain("public partial class MyServiceClient : EntryEndpoint, IMyServiceClient", "the base class is still needed")
           .And.NotContain("public MyServiceClient(", "the consumer supplies the constructors in their own partial class");
    }

    [Fact]
    public void UsesDtoNamespace()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .WithMetadata("DtoNamespace", "MyNamespace.Dtos")
                    .Run();

        HintNames(result).Should().Contain([
            "sample-v3.MyNamespace.MyServiceClient.g.cs",
            "sample-v3.MyNamespace.Dtos.Contact.g.cs"
        ]);
    }

    [Fact]
    public void IsIncremental()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3).ForService("MyService").Run(twice: true);

        result.TrackedSteps["SourceOutput"]
              .SelectMany(x => x.Outputs)
              .Should().OnlyContain(x => x.Reason == IncrementalStepRunReason.Cached);
    }

    private static IEnumerable<string> HintNames(GeneratorRunResult result)
        => result.GeneratedSources.Select(x => x.HintName);

    private static string Source(GeneratorRunResult result, string hintName)
        => result.GeneratedSources.Single(x => x.HintName == hintName).SourceText.ToString();
}
