---
uid: TypedRest.CodeGeneration.TypeScript
summary: Generates TypeScript source code for [TypedRest for TypeScript](https://github.com/TypedRest/TypedRest-TypeScript) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.
---
> [!NOTE]
> NuGet package: [TypedRest.CodeGeneration.TypeScript](https://www.nuget.org/packages/TypedRest.CodeGeneration.TypeScript/)

## Usage

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

foreach (var file in doc.GenerateTypedRestTypeScript(new TypeScriptGenerationOptions("MyService")
{
    DtoNamespace = "dtos",
    GenerateDtos = true
}))
    file.WriteToDirectory("myclient/");
```

<xref:TypedRest.CodeGeneration.TypeScript.OpenApiDocumentExtensions.GenerateTypedRestTypeScript*> uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths if there is no such extension.

## Differences from the C# generator

TypedRest for TypeScript has no reactive endpoints, so the `polling`, `streaming`, `sse` and `streaming-collection` kinds are degraded to their closest non-reactive equivalent and reported through the <xref:TypedRest.CodeGeneration.Generation.IGenerationLog>.

TypeScript is structurally typed and TypedRest for TypeScript has no endpoint interfaces, so <xref:TypedRest.CodeGeneration.Generation.ClientGenerationOptions.GenerateInterfaces> has no effect.

DTO properties keep the exact name used on the wire, because TypedRest for TypeScript deserializes with `JSON.parse()` and a cast and therefore has no way to map a property to a differently named field.

## Extension points

<xref:TypedRest.CodeGeneration.TypeScript.OpenApiDocumentExtensions.GenerateTypedRestTypeScript*> takes an optional <xref:TypedRest.CodeGeneration.Patterns.PatternRegistry> controlling what is inferred, and an optional <xref:TypedRest.CodeGeneration.TypeScript.Endpoints.BuilderRegistry> controlling what is emitted:

```csharp
var files = doc.GenerateTypedRestTypeScript(options, log, patterns, builders);
```

Implement <xref:TypedRest.CodeGeneration.TypeScript.Endpoints.IBuilder%601> to change the code emitted for an endpoint kind. A <xref:TypedRest.CodeGeneration.TypeScript.Endpoints.BuilderRegistry> holds at most one builder per kind, so replacing a built-in one means composing the registry yourself rather than adding to <xref:TypedRest.CodeGeneration.TypeScript.Endpoints.BuilderRegistry.Default?displayProperty=nameWithType>.

To change how types and properties are named, derive from <xref:TypedRest.CodeGeneration.TypeScript.NamingStrategy> and pass it to <xref:TypedRest.CodeGeneration.TypeScript.OpenApiDocumentExtensions.GenerateTypedRestTypeScriptEndpoints*>.

## API
