using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

public class NamingStrategyFacts
{
    private readonly INamingStrategy _namingStrategy = new NamingStrategy("MyService", "", "dtos");

    [Theory]
    [InlineData("", "")]
    [InlineData("my_property", "myProperty")]
    [InlineData("my-property", "myProperty")]
    [InlineData("myProperty", "myProperty")]
    [InlineData("MyProperty", "myProperty")]
    [InlineData("1st", "_1st")]
    public void NamesGettersInCamelCase(string key, string expected)
        => _namingStrategy.Property(key).Should().Be(expected);

    [Fact]
    public void NamesEntryEndpointAfterService()
    {
        var identifier = _namingStrategy.EndpointType("entry", new EntryEndpoint());

        identifier.Name.Should().Be("MyServiceClient");
        identifier.Module!.Specifier.Should().Be("MyServiceClient");
    }

    [Fact]
    public void NamesIndexerEndpointAfterSingularKey()
        => _namingStrategy.EndpointType("contacts", new IndexerEndpoint())
                          .Name.Should().Be("ContactCollectionEndpoint");

    [Fact]
    public void PrefixesEndpointTypes()
        => _namingStrategy.EndpointType("settings", new Endpoint(), "users")
                          .Name.Should().Be("UsersSettingsEndpoint");

    [Fact]
    public void PutsDtosInTheirOwnDirectory()
    {
        var identifier = _namingStrategy.DtoType("Contact");

        identifier.Name.Should().Be("Contact");
        identifier.Module!.Specifier.Should().Be("dtos/Contact");
    }

    [Fact]
    public void TurnsDottedDtoKeysIntoSubdirectories()
    {
        var identifier = _namingStrategy.DtoType("foo.bar.Baz");

        identifier.Name.Should().Be("Baz");
        identifier.Module!.Specifier.Should().Be("dtos/foo/bar/Baz");
    }

    [Fact]
    public void TurnsDottedNamespacesIntoDirectories()
        => new NamingStrategy("MyService", "MyCompany.MyService", "dtos")
          .EndpointType("entry", new EntryEndpoint())
          .Module!.Specifier.Should().Be("myCompany/myService/MyServiceClient");

    [Theory]
    [InlineData("string", null, "string")]
    [InlineData("string", "uri", "string")]
    [InlineData("string", "uuid", "string")]
    // There is no converter, so a date-time arrives as the string JSON.parse() produced
    [InlineData("string", "date-time", "string")]
    [InlineData("string", "byte", "string")]
    [InlineData("integer", null, "number")]
    [InlineData("integer", "int64", "number")]
    [InlineData("number", "double", "number")]
    [InlineData("number", "decimal", "number")]
    [InlineData("boolean", null, "boolean")]
    public void MapsPrimitiveTypes(string type, string? format, string expected)
        => _namingStrategy.TypeFor(new OpenApiSchema {Type = type, Format = format})
                          .ToTypeExpression().Should().Be(expected);

    [Fact]
    public void MapsArrays()
        => _namingStrategy.TypeFor(new OpenApiSchema {Type = "array", Items = new OpenApiSchema {Type = "string"}})
                          .ToTypeExpression().Should().Be("string[]");

    [Fact]
    public void MapsAdditionalPropertiesToRecord()
        => _namingStrategy.TypeFor(new OpenApiSchema {AdditionalProperties = new OpenApiSchema {Type = "integer"}})
                          .ToTypeExpression().Should().Be("Record<string, number>");

    [Fact]
    public void MapsReferencesToDtoTypes()
        => _namingStrategy.TypeFor(Sample.ContactSchema)
                          .Should().BeEquivalentTo(new TsIdentifier(TsModule.Generated("dtos/Contact"), "Contact"));

    [Fact]
    public void FallsBackToUnknown()
        => _namingStrategy.TypeFor(new OpenApiSchema()).ToTypeExpression().Should().Be("unknown");

    [Fact]
    public void HonorsConfiguredUntypedFallback()
        => new NamingStrategy("MyService", "", "dtos", TsIdentifier.Any)
          .TypeFor(new OpenApiSchema()).ToTypeExpression().Should().Be("any");

    [Fact]
    public void MapsNullableToUnionWithNull()
        => _namingStrategy.TypeFor(new OpenApiSchema {Type = "string", Nullable = true})
                          .ToTypeExpression().Should().Be("string | null");
}
