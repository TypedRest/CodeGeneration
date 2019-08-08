using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace TypedRest.OpenApi
{
    public class SerializationFacts
    {
        [Fact]
        public void CanSerializeV2()
        {
            Sample.Doc.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)
                .Should().Be(Sample.YamlV2);
        }

        [Fact]
        public void CanDeserializeV2()
        {
            var reader = new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest());
            reader.Read(Sample.YamlV2, out _)
                  .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void CanSerializeV3()
        {
            Sample.Doc.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)
                .Should().Be(Sample.YamlV3);
        }

        [Fact]
        public void CanDeserializeV3()
        {
            var reader = new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest());
            reader.Read(Sample.YamlV3, out _)
                  .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }
    }
}
