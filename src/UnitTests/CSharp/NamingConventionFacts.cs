using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.CSharp
{
    public class NamingConventionFacts
    {
        private readonly INamingConvention _namingConvention = new NamingConvention("MyNamespace", "MyService");

        [Fact]
        public void PropertyEmptyString()
        {
            _namingConvention
               .Property("")
               .Should().Be("");
        }

        [Fact]
        public void PropertyFromSnakeCase()
        {
            _namingConvention
               .Property("my_property")
               .Should().Be("MyProperty");
        }

        [Fact]
        public void PropertyFromCamelCase()
        {
            _namingConvention
               .Property("myProperty")
               .Should().Be("MyProperty");
        }

        [Fact]
        public void EndpointFromTypeSnakeCase()
        {
            _namingConvention
               .EndpointType("my_type", new Endpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
        }

        [Fact]
        public void EndpointTypeFromCamelCase()
        {
            _namingConvention
               .EndpointType("myType", new Endpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
        }

        [Fact]
        public void EndpointTypeFromPlural()
        {
            _namingConvention
               .EndpointType("myTypes", new IndexerEndpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeCollectionEndpoint"));
        }

        [Fact]
        public void DtoTypeFromTypeSnakeCase()
        {
            _namingConvention
               .DtoType("my_type")
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyType"));
        }

        [Fact]
        public void DtoTypeFromCamelCase()
        {
            _namingConvention
               .DtoType("myType")
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyType"));
        }

        [Fact]
        public void TypeForString()
        {
            _namingConvention
               .TypeFor(new OpenApiSchema {Type = "string"})
               .Should().BeEquivalentTo(CSharpIdentifier.String);
        }

        [Fact]
        public void TypeForArrayOfUri()
        {
            _namingConvention
               .TypeFor(new OpenApiSchema {Type = "array", Items = new OpenApiSchema {Type = "string", Format = "uri"}})
               .Should().BeEquivalentTo(CSharpIdentifier.ListOf(CSharpIdentifier.Uri));
        }

        [Fact]
        public void TypeForReference()
        {
            _namingConvention
               .TypeFor(new OpenApiSchema {Reference = new OpenApiReference {Id = "myType"}})
               .Should().BeEquivalentTo(_namingConvention.DtoType("myType"));
        }
    }
}
