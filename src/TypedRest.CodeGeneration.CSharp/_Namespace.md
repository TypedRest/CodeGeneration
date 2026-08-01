---
uid: TypedRest.CodeGeneration.CSharp
summary: Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.
---
> [!NOTE]
> NuGet package: [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/)

## Usage

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

<xref:TypedRest.CodeGeneration.CSharp.OpenApiDocumentExtensions.GenerateTypedRest*> uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths if there is no such extension.

## Extension points

<xref:TypedRest.CodeGeneration.CSharp.OpenApiDocumentExtensions.GenerateTypedRest*> takes an optional <xref:TypedRest.CodeGeneration.Patterns.PatternRegistry> controlling what is inferred, and an optional <xref:TypedRest.CodeGeneration.CSharp.Endpoints.BuilderRegistry> controlling what is emitted:

```csharp
var types = doc.GenerateTypedRest(options, patterns, builders);
```

Implement <xref:TypedRest.CodeGeneration.CSharp.Endpoints.IBuilder%601> to change the code emitted for an endpoint kind. A <xref:TypedRest.CodeGeneration.CSharp.Endpoints.BuilderRegistry> holds at most one builder per kind, so replacing a built-in one means composing the registry yourself rather than adding to <xref:TypedRest.CodeGeneration.CSharp.Endpoints.BuilderRegistry.Default?displayProperty=nameWithType>.

To change how types and properties are named, derive from <xref:TypedRest.CodeGeneration.CSharp.NamingStrategy> and pass it to <xref:TypedRest.CodeGeneration.CSharp.OpenApiDocumentExtensions.GenerateTypedRestEndpoints*>.

## API
