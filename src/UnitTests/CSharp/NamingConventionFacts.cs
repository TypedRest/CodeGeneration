using FluentAssertions;
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
    }
}
