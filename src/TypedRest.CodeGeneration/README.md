# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation

Parses [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in the described paths.

    dotnet add package TypedRest.CodeGeneration

This library only models the endpoints. To turn them into source code, combine it with [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/). If you just want to generate a client for your API, use the [source generator](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp.SourceGenerator/) or the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) instead; both are built on this library.

## Usage

Register the `x-typedrest` extension parser when reading a document, then either read the endpoints the document already describes or infer them from its paths:

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

EntryEndpoint endpoints = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns();
```

`MatchTypedRestPatterns()` arranges the paths into a tree and matches each node against the patterns in `PatternRegistry.Default`. Write the result back into the document with `doc.SetTypedRest(endpoints)` to persist or hand-edit it.

Matching is deliberately conservative: a pattern only claims a path when the endpoint it produces is an accurate description of it. `ElementPattern`, for instance, models a single type that is both read and written, so it declines paths whose `PUT`/`PATCH` takes a different schema than the `GET` returns, rather than generating an endpoint that would send the wrong request body. Those paths fall through to a plain endpoint that still exposes its children, leaving the operations to be written by hand.

## Extension points

Implement `IPattern` and add it to a `PatternRegistry` to recognize path shapes of your own:

```csharp
var patterns = PatternRegistry.Default.Add(new MyPattern());
var endpoints = doc.MatchTypedRestPatterns(patterns);
```

Patterns added later take precedence; the first one that matches a node wins.

Implement `IEndpoint` and add it to an `EndpointRegistry` to introduce a new endpoint kind, then pass the registry to `AddTypedRest()` so that `x-typedrest` extensions using that kind can be parsed.

## Related packages

- [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/) builds on this library, to generate C# source code for TypedRest clients.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
