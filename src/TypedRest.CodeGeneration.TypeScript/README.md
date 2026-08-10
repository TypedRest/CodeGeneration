# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for TypeScript

Generates TypeScript source code for [TypedRest for TypeScript](https://github.com/TypedRest/TypedRest-TypeScript) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.

    dotnet add package TypedRest.CodeGeneration.TypeScript

Use this to build your own code generator. If you just want to generate a client for your API, use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) instead; it is built on this library.

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

`GenerateTypedRestTypeScript()` uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths using [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) if there is no such extension.

The generated code imports from the [`typedrest`](https://www.npmjs.com/package/typedrest) package, so consumers need to run:

    npm install typedrest

## Output

One file per generated type, plus an `index.ts` re-exporting all of them. `Namespace` and `DtoNamespace` are directories relative to the output directory, defaulting to the root and `dtos` respectively.

Endpoints become classes deriving from the TypedRest endpoint types, exposing their children as getters. DTOs become interfaces, and schemas with an `enum` become literal union type aliases.

## Extension points

`GenerateTypedRestTypeScript()` takes an optional `PatternRegistry` controlling what is inferred, and an optional `BuilderRegistry` controlling what is emitted:

```csharp
var files = doc.GenerateTypedRestTypeScript(options, log, patterns, builders);
```

Implement `IBuilder<TEndpoint>` to change the code emitted for an endpoint kind. A `BuilderRegistry` holds at most one builder per kind, so replacing a built-in one means composing the registry yourself rather than adding to `BuilderRegistry.Default`.

To change how types and properties are named, derive from `NamingStrategy` and pass it to `GenerateTypedRestTypeScriptEndpoints()`.

## Related packages

- [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) is the basis of this library. It parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.
- [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/) does the same for C#.
- [typedrest-codegen](https://www.nuget.org/packages/typedrest-codegen/) is a command-line tool that builds on this library and writes the generated code to disk.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
