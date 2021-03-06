using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
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
            Deserialize(Sample.YamlV2)
               .Should().BeEquivalentTo(Sample.Doc, options =>
                    options.IncludingAllRuntimeProperties()
                           .Excluding(info => info.SelectedMemberPath.Contains("RequestBody.Extensions")));
        }

        [Fact]
        public void CanDeserializeV3()
        {
            Deserialize(Sample.YamlV3)
               .Should().BeEquivalentTo(Sample.Doc, options =>
                    options.IncludingAllRuntimeProperties());
        }

        private OpenApiDocument Deserialize(string data)
        {
            var reader = new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest());
            var doc = reader.Read(data, out _);
            doc.GetTypedRest(); // Resolves references
            return doc;
        }
    }
}
