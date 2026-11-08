namespace TypedRest.CodeGeneration;

public class SerializationFacts
{
    [Fact]
    public void CanSerializeV2()
    {
        Serialize(OpenApiSpecVersion.OpenApi2_0)
           .Should().Be(Sample.YamlV2);
    }

    [Fact]
    public void CanSerializeV3()
    {
        Serialize(OpenApiSpecVersion.OpenApi3_0)
           .Should().Be(Sample.YamlV3);
    }

    [Fact]
    public void CanDeserializeV2()
    {
        Deserialize(Sample.YamlV2)
           .Should().BeEquivalentTo(Sample.Doc, options =>
                options.IncludingAllRuntimeProperties()
                       .Excluding(info => info.Path.Contains("HashCode"))
                       .Excluding(info => info.Path.Contains("HostDocument"))
                       .Excluding(info => info.Path.Contains("BaseUri"))
                       .Excluding(info => info.Path.Contains("RequestBody.Extensions"))
                       // Body parameters are converted to a request body, leaving an empty parameter list behind
                       .Excluding(info => info.Path.EndsWith("Operations[POST].Parameters"))
                       .Excluding(info => info.Path.Contains("Content")));
    }

    [Fact]
    public void CanDeserializeV3()
    {
        Deserialize(Sample.YamlV3)
           .Should().BeEquivalentTo(Sample.Doc, options =>
                options.IncludingAllRuntimeProperties()
                       .Excluding(info => info.Path.Contains("HostDocument"))
                       .Excluding(info => info.Path.Contains("BaseUri")));
    }

    [Theory]
    [InlineData("3.1.1", OpenApiSpecVersion.OpenApi3_1)]
    [InlineData("3.2.0", OpenApiSpecVersion.OpenApi3_2)]
    [InlineData("3.2.1", OpenApiSpecVersion.OpenApi3_2)]
    public void CanDeserializeV31AndV32(string version, OpenApiSpecVersion expectedSpecVersion)
    {
        var (doc, diagnostic) = OpenApiDocumentReader.Parse(Sample.YamlV3.Replace("openapi: 3.0.4", $"openapi: {version}"));

        diagnostic!.Errors.Should().BeEmpty();
        diagnostic.SpecificationVersion.Should().Be(expectedSpecVersion);
        doc!.GetTypedRest(); // Resolves references
        doc.Should().BeEquivalentTo(Sample.Doc, options =>
            options.IncludingAllRuntimeProperties()
                   .Excluding(info => info.Path.Contains("HostDocument"))
                   .Excluding(info => info.Path.Contains("BaseUri")));
    }

    private static string Serialize(OpenApiSpecVersion version)
        => Sample.Doc.SerializeAsYamlAsync(version, TestContext.Current.CancellationToken).GetAwaiter().GetResult().TrimEnd();

    private static OpenApiDocument Deserialize(string data)
    {
        var doc = OpenApiDocumentReader.Parse(data).Document!;
        doc.GetTypedRest(); // Resolves references
        return doc;
    }
}
