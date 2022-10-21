using FluentAssertions;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using Xunit;

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
    public void TypeForString()
    {
        _namingStrategy
           .TypeFor(new OpenApiSchema {Type = "string"})
           .Should().BeEquivalentTo(CSharpIdentifier.String);
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
