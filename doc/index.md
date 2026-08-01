---
title: Home
---

# TypedRest Code Generation

This website documents the API of the TypedRest Code Generation libraries. Use them to build your own tools that infer [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in OpenAPI/Swagger documents and generate client source code.

If you just want to generate a client for your API, use the [source generator](https://typedrest.net/code-generation/source-generator/) or the [command-line tool](https://typedrest.net/code-generation/cli/) instead. Both are built on these libraries.

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

foreach (var type in doc.GenerateTypedRest(new GenerationOptions("MyService")
{
    Namespace = "MyCompany.MyService",
    GenerateInterfaces = true,
    GenerateDtos = true
}))
    type.WriteToDirectory("myclient/");
```

**NuGet packages**

[TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/)  
Parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.  
Start at <xref:TypedRest.CodeGeneration.OpenApiDocumentExtensions.MatchTypedRestPatterns*>. The inferred structure is a tree of <xref:TypedRest.CodeGeneration.Endpoints.IEndpoint>s, built by the <xref:TypedRest.CodeGeneration.Patterns.IPattern>s in a <xref:TypedRest.CodeGeneration.Patterns.PatternRegistry>.

[TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/)  
Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from OpenAPI/Swagger documents.  
Start at <xref:TypedRest.CodeGeneration.CSharp.OpenApiDocumentExtensions.GenerateTypedRest*>, configured via <xref:TypedRest.CodeGeneration.CSharp.GenerationOptions>. Change the emitted code with the builders in a <xref:TypedRest.CodeGeneration.CSharp.Endpoints.BuilderRegistry>, or the generated names with a <xref:TypedRest.CodeGeneration.CSharp.NamingStrategy>.
