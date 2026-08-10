namespace TypedRest.CodeGeneration.TypeScript.Model;

public class TsModuleFacts
{
    [Theory]
    [InlineData("ContactEndpoint.ts", "dtos/Contact", "./dtos/Contact")]
    [InlineData("dtos/Contact.ts", "ContactEndpoint", "../ContactEndpoint")]
    [InlineData("dtos/Contact.ts", "dtos/Note", "./Note")]
    [InlineData("SampleClient.ts", "ContactEndpoint", "./ContactEndpoint")]
    [InlineData("a/b/X.ts", "a/c/Y", "../c/Y")]
    [InlineData("a/b/X.ts", "Y", "../../Y")]
    [InlineData("index.ts", "dtos/sub/Deep", "./dtos/sub/Deep")]
    public void ResolvesRelativeSpecifiers(string fromFilePath, string specifier, string expected)
        => TsModule.Generated(specifier).RelativeTo(fromFilePath).Should().Be(expected);

    [Fact]
    public void LeavesExternalSpecifiersAlone()
        => TsModule.External("typedrest/endpoints/generic").RelativeTo("dtos/Contact.ts")
                   .Should().Be("typedrest/endpoints/generic");

    [Fact]
    public void DerivesFilePathFromSpecifier()
        => TsModule.Generated("dtos/Contact").FilePath.Should().Be("dtos/Contact.ts");

    [Fact]
    public void RejectsFilePathForExternalModules()
        => new Func<string>(() => TsModule.External("typedrest").FilePath)
          .Should().Throw<InvalidOperationException>();

    [Fact]
    public void ComparesByValue()
    {
        TsModule.Generated("dtos/Contact").Should().Be(TsModule.Generated("dtos/Contact"));
        TsModule.Generated("typedrest").Should().NotBe(TsModule.External("typedrest"));
    }
}
