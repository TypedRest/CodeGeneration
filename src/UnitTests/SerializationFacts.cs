using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace TypedRest.OpenApi
{
    public class SerializationFacts
    {
        [Fact]
        public void CanSerializeV2()
        {
            Serialize(Sample.Doc, OpenApiSpecVersion.OpenApi2_0)
                .Should().Be(Sample.YamlV2);
        }

        [Fact]
        public void CanSerializeV3()
        {
            Serialize(Sample.Doc, OpenApiSpecVersion.OpenApi3_0)
               .Should().Be(Sample.YamlV3);
        }

        private static string Serialize(OpenApiDocument doc, OpenApiSpecVersion specVersion)
            => doc.Serialize(specVersion, OpenApiFormat.Yaml);

        [Fact]
        public void CanDeserializeV2()
        {
            Deserialize(Sample.YamlV2)
               .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void CanDeserializeV3()
        {
            Deserialize(Sample.YamlV3)
               .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }

        private static OpenApiDocument Deserialize(string yaml)
            => new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest()).Read(yaml, out _).ResolveTypedRestReferences();
    }
}
