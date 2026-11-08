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
    public void GeneratesFromOpenApi32()
    {
        var result = new GeneratorTestHarness("""
                         openapi: 3.2.1
                         info:
                           title: Pets
                           version: 1.0.0
                         paths:
                           /pets:
                             get:
                               responses:
                                 '200':
                                   description: OK
                                   content:
                                     application/json:
                                       schema:
                                         type: array
                                         items:
                                           $ref: '#/components/schemas/Pet'
                           /pets/{id}:
                             get:
                               parameters:
                                 - name: id
                                   in: path
                                   required: true
                                   schema:
                                     type: string
                               responses:
                                 '200':
                                   description: OK
                                   content:
                                     application/json:
                                       schema:
                                         $ref: '#/components/schemas/Pet'
                         components:
                           schemas:
                             Pet:
                               type: object
                               required: [name]
                               properties:
                                 name:
                                   type: string
                                 nickname:
                                   type: [string, 'null']
                                 kind:
                                   type: string
                                   enum: [cat, dog]
                         """, path: "pets.yml")
                    .ForService("Pets")
                    .WithMetadata("Namespace", "MyNamespace")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        Source(result, "pets.MyNamespace.PetsClient.g.cs")
           .Should().Contain("ICollectionEndpoint<Pet> Pets => new CollectionEndpoint<Pet>(this, relativeUri: \"./pets\")");
        Source(result, "pets.MyNamespace.Pet.g.cs")
           .Should().Contain("public required string Name { get; set; }")
           .And.Contain("public string? Nickname { get; set; }", "type: [string, 'null'] is how OpenAPI 3.1+ expresses nullability")
           .And.Contain("public PetKind? Kind { get; set; }");
        Source(result, "pets.MyNamespace.PetKind.g.cs")
           .Should().Contain("Cat,").And.Contain("Dog");
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

        result.Diagnostics.Select(x => x.Id).Should().BeEquivalentTo(["TRCG004"]);
        result.GeneratedSources.Should().BeEmpty();
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
    public void ConfiguresSystemTextJsonInEntryConstructor()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .WithMetadata("Serializer", "system-text-json")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        Source(result, "sample-v3.MyNamespace.MyServiceClient.g.cs")
           .Should().Contain("using TypedRest.Serializers;")
           .And.Contain("public MyServiceClient(Uri uri) : base(uri, serializer: new SystemTextJsonSerializer())",
                "the DTOs are annotated for System.Text.Json, so the client has to serialize with it");
    }

    [Fact]
    public void UsesDefaultSerializerInEntryConstructorForNewtonsoft()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3)
                    .ForService("MyService")
                    .WithMetadata("Namespace", "MyNamespace")
                    .WithMetadata("Serializer", "newtonsoft")
                    .Run();

        result.Diagnostics.Should().BeEmpty();
        Source(result, "sample-v3.MyNamespace.MyServiceClient.g.cs")
           .Should().Contain("public MyServiceClient(Uri uri) : base(uri)");
    }

    [Fact]
    public void StampsGeneratedCodeWithGeneratorVersion()
    {
        var result = new GeneratorTestHarness(Sample.YamlV3).ForService("MyService").Run();

        string version = typeof(JsonAttributes).Assembly.GetName().Version!.ToString(3);
        Source(result, "sample-v3.MyService.MyServiceClient.g.cs")
           .Should().Contain($"[GeneratedCode(\"TypedRest.CodeGeneration\", \"{version}");
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
