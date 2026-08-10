# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for C#

Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.

    dotnet add package TypedRest.CodeGeneration.CSharp

Use this to build your own code generator. If you just want to generate a client for your API, use the [source generator](https://www.nuget.org/packages/TypedRest.SourceGenerator/) or the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) instead; both are built on this library.

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

`GenerateTypedRest()` uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths using [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) if there is no such extension.

The generated code derives from the [`TypedRest`](https://www.nuget.org/packages/TypedRest/) package, so consumers need to run:

    dotnet add package TypedRest

## Extension points

`GenerateTypedRest()` takes an optional `PatternRegistry` controlling what is inferred, and an optional `BuilderRegistry` controlling what is emitted:

```csharp
var types = doc.GenerateTypedRest(options, patterns, builders);
```

Implement `IBuilder<TEndpoint>` to change the code emitted for an endpoint kind. A `BuilderRegistry` holds at most one builder per kind, so replacing a built-in one means composing the registry yourself rather than adding to `BuilderRegistry.Default`.

To change how types and properties are named, derive from `NamingStrategy` and pass it to `GenerateTypedRestEndpoints()`.

## Related packages

- [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) is the basis of this library. It parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.
- [TypedRest.SourceGenerator](https://www.nuget.org/packages/TypedRest.SourceGenerator/) builds on this library to generate clients during compilation.
- [typedrest-codegen](https://www.nuget.org/packages/typedrest-codegen/) is a command-line tool that builds on this library and writes the generated code to disk.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
