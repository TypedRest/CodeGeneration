using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Xunit;

namespace TypedRest.CodeGeneration
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
        public void CanSerializeV3()
        {
            Sample.Doc.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)
                  .Should().Be(Sample.YamlV3);
        }

        [Fact]
        public void CanDeserializeV2()
        {
            OpenApiDocumentExtensions.ReadWithTypedRest(Sample.YamlV2)
                                     .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void CanDeserializeV3()
        {
            OpenApiDocumentExtensions.ReadWithTypedRest(Sample.YamlV3)
                                     .Should().BeEquivalentTo(Sample.Doc, options => options.IncludingAllRuntimeProperties());
        }
    }
}
