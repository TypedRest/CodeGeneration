using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp;

public class NamingStrategyFacts
{
    private readonly INamingStrategy _namingStrategy = new NamingStrategy("MyService", "MyNamespace", "MyNamespace");

    [Fact]
    public void PropertyEmptyString()
    {
        _namingStrategy
           .Property("")
           .Should().Be("");
    }

    [Fact]
    public void PropertyFromSnakeCase()
    {
        _namingStrategy
           .Property("my_property")
           .Should().Be("MyProperty");
    }

    [Fact]
    public void PropertyFromCamelCase()
    {
        _namingStrategy
           .Property("myProperty")
           .Should().Be("MyProperty");
    }

    [Theory]
    [InlineData("n/a", "NA")]
    [InlineData("a+b", "AB")]
    [InlineData("$ref", "Ref")]
    [InlineData("my property", "MyProperty")]
    public void PropertyStripsCharactersInvalidInIdentifiers(string key, string expected)
    {
        _namingStrategy
           .Property(key)
           .Should().Be(expected);
    }

    [Fact]
    public void PropertyEscapesLeadingDigit()
    {
        _namingStrategy
           .Property("1st")
           .Should().Be("_1st");
    }

    [Fact]
    public void EndpointFromTypeSnakeCase()
    {
        _namingStrategy
           .EndpointType("my_type", new Endpoint())
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
    }

    [Fact]
    public void EndpointTypeFromCamelCase()
    {
        _namingStrategy
           .EndpointType("myType", new Endpoint())
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
    }

    [Fact]
    public void EndpointTypeFromPlural()
    {
        _namingStrategy
           .EndpointType("myTypes", new IndexerEndpoint())
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeCollectionEndpoint"));
    }

    [Fact]
    public void DtoTypeFromTypeSnakeCase()
    {
        _namingStrategy
           .DtoType("my_type")
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyType"));
    }

    [Fact]
    public void DtoTypeFromCamelCase()
    {
        _namingStrategy
           .DtoType("myType")
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyType"));
    }

    [Fact]
    public void DtoTypeFromDottedKey()
    {
        _namingStrategy
           .DtoType("foo.bar.Baz")
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace.Foo.Bar", "Baz"));
    }

    [Fact]
    public void DtoTypeFromSlashedKey()
    {
        _namingStrategy
           .DtoType("foo/bar/Baz")
           .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace.Foo.Bar", "Baz"));
    }

    [Fact]
    public void TypeForString()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "string"})
           .Should().BeEquivalentTo(CSharpIdentifier.String);
    }

    [Fact]
    public void TypeForNullableString()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "string", Nullable = true})
           .Should().BeEquivalentTo(CSharpIdentifier.String.ToNullable());
    }

    [Fact]
    public void TypeForNullableStringWithoutNullableReferenceTypes()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "array", Items = new OpenApiSchema {Type = "string", Nullable = true}}, nullableReferenceTypes: false)
           .Should().BeEquivalentTo(CSharpIdentifier.ListOf(CSharpIdentifier.String));
    }

    [Fact]
    public void TypeForNullableIntegerWithoutNullableReferenceTypes()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "integer", Nullable = true}, nullableReferenceTypes: false)
           .Should().BeEquivalentTo(CSharpIdentifier.Int.ToNullable());
    }

    [Theory]
    [InlineData("string", "uuid", "System", "Guid")]
    [InlineData("string", "guid", "System", "Guid")]
    [InlineData("string", "date-time", "System", "DateTimeOffset")]
    [InlineData("string", "date", "System", "DateTime")]
    [InlineData("string", "time", "System", "TimeSpan")]
    [InlineData("string", "duration", "System", "TimeSpan")]
    [InlineData("number", "decimal", null, "decimal")]
    public void TypeForFormat(string type, string format, string? ns, string name)
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = type, Format = format})
           .Should().BeEquivalentTo(new CSharpIdentifier(ns, name));
    }

    [Fact]
    public void TypeForNullableGuidWithoutNullableReferenceTypes()
    {
        // Formats that map to value types stay nullable even without nullable reference types
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "string", Format = "uuid", Nullable = true}, nullableReferenceTypes: false)
           .Should().BeEquivalentTo(new CSharpIdentifier("System", "Guid").ToNullable());
    }

    [Fact]
    public void TypeForInteger()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "integer"})
           .Should().BeEquivalentTo(CSharpIdentifier.Int);
    }

    [Fact]
    public void TypeForArrayOfUri()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "array", Items = new OpenApiSchema {Type = "string", Format = "uri"}})
           .Should().BeEquivalentTo(CSharpIdentifier.ListOf(CSharpIdentifier.Uri));
    }

    [Fact]
    public void TypeForReference()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Reference = new OpenApiReference {Id = "myType"}})
           .Should().BeEquivalentTo(_namingStrategy.DtoType("myType"));
    }
}
