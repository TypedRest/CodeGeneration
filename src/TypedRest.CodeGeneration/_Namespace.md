---
uid: TypedRest.CodeGeneration
summary: Parses [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in the described paths.
---
> [!NOTE]
> NuGet package: [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/)

## Usage

Register the `x-typedrest` extension parser when reading a document, then either read the endpoints the document already describes or infer them from its paths:

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

EntryEndpoint endpoints = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns();
```

<xref:TypedRest.CodeGeneration.OpenApiDocumentExtensions.MatchTypedRestPatterns*> arranges the paths into a tree and matches each node against the patterns in <xref:TypedRest.CodeGeneration.Patterns.PatternRegistry.Default?displayProperty=nameWithType>. Write the result back into the document with <xref:TypedRest.CodeGeneration.OpenApiDocumentExtensions.SetTypedRest*> to persist or hand-edit it.

## Extension points

Implement <xref:TypedRest.CodeGeneration.Patterns.IPattern> and add it to a <xref:TypedRest.CodeGeneration.Patterns.PatternRegistry> to recognize path shapes of your own:

```csharp
var patterns = PatternRegistry.Default.Add(new MyPattern());
var endpoints = doc.MatchTypedRestPatterns(patterns);
```

Patterns added later take precedence; the first one that matches a node wins.

Implement <xref:TypedRest.CodeGeneration.Endpoints.IEndpoint> and add it to an <xref:TypedRest.CodeGeneration.Endpoints.EndpointRegistry> to introduce a new endpoint kind, then pass the registry to <xref:TypedRest.CodeGeneration.OpenApiReaderSettingsExtensions.AddTypedRest*> so that `x-typedrest` extensions using that kind can be parsed.

## API
