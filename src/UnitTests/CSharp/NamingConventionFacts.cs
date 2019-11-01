using FluentAssertions;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.CSharp
{
    public class NamingConventionFacts
    {
        [Fact]
        public void PropertyFromSnakeCase()
        {
            new NamingConvention("MyNamespace")
               .Property("my_property")
               .Should().Be("MyProperty");
        }

        [Fact]
        public void PropertyFromCamelCase()
        {
            new NamingConvention("MyNamespace")
               .Property("myProperty")
               .Should().Be("MyProperty");
        }

        [Fact]
        public void EndpointFromTypeSnakeCase()
        {
            new NamingConvention("MyNamespace")
               .EndpointType("my_type", new Endpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
        }

        [Fact]
        public void EndpointTypeFromCamelCase()
        {
            new NamingConvention("MyNamespace")
               .EndpointType("myType", new Endpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeEndpoint"));
        }

        [Fact]
        public void EndpointTypeFromPlural()
        {
            new NamingConvention("MyNamespace")
               .EndpointType("myTypes", new IndexerEndpoint())
               .Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyTypeCollectionEndpoint"));
        }
    }
}
