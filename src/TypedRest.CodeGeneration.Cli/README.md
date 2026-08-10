# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation CLI

Command-line tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients.

Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-codegen

If you build your client with the .NET SDK, consider the [source generator](https://www.nuget.org/packages/TypedRest.SourceGenerator/) instead. It runs the same generator during compilation, without writing files to your source tree.

For a walkthrough see the **[usage guide](https://typedrest.net/code-generation/cli/)**.

## `generate`

Generates a TypedRest client.

    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-interfaces --generate-dtos

| Option                            | Description                                                                                                                        | Default                |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ | ---------------------- |
| `-f`, `--file` (required)         | The path to the Swagger or OpenAPI spec file. Use `-` to read from standard input.                                                 |                        |
| `-o`, `--output` (required)       | The directory to write the generated source code to.                                                                               |                        |
| `-s`, `--service-name` (required) | The service name to use for the entry endpoint.                                                                                    |                        |
| `-l`, `--language`                | The language to generate: `csharp` or `typescript`.                                                                                | `csharp`               |
| `-n`, `--namespace`               | The C# namespace for the endpoints, or the directory for TypeScript.                                                               | the service name       |
| `--dto-namespace`                 | The C# namespace for the DTOs, or the directory for TypeScript.                                                                    | see below              |
| `--generate-interfaces`           | Also generate interfaces for the endpoints. **C# only.**                                                                           | off                    |
| `--generate-dtos`                 | Also generate DTOs for the schemas in the document.                                                                                | off                    |
| `--generate-entry-constructor`    | Give the entry endpoint a constructor taking the base URI. Pass `false` to write your own in a partial class. **C# only.**          | on                     |
| `--lang-version`                  | The minimum C# version the generated code must compile with, using the same values as the MSBuild `LangVersion` property. **C# only.** | `latest`            |

### C#

The generated code derives from the [`TypedRest`](https://www.nuget.org/packages/TypedRest/) package, so run `dotnet add package TypedRest` in the consuming project.

Unlike the [source generator](https://www.nuget.org/packages/TypedRest.SourceGenerator/), interfaces and DTOs are opt-in here. Generated endpoints reference the DTO types by name, so without `--generate-dtos` you have to provide those types yourself.

Use `--generate-entry-constructor false` when the entry endpoint needs a custom error handler or default headers. The class and its base type are still generated, but the constructor is left for you to write in a partial class.

### TypeScript

    typedrest-codegen generate -l typescript -f myapi.yml -o src/myclient/ -s MyService --generate-dtos

The generated code imports from the [`typedrest`](https://www.npmjs.com/package/typedrest) package, so run `npm install typedrest` in the consuming project.

Each generated type gets its own file, plus an `index.ts` re-exporting all of them. Here `--namespace` and `--dto-namespace` are directories relative to `--output` rather than namespaces, defaulting to the output directory itself and to `dtos`. Dotted values such as `MyCompany.MyService` become nested directories.

Endpoints become classes deriving from the TypedRest endpoint types and exposing their children as getters. DTOs become interfaces whose properties keep the exact name used on the wire, because TypedRest for TypeScript deserializes with `JSON.parse()` and a cast and so has no way to map a property to a differently named field. Schemas with an `enum` become literal union type aliases.

## `pattern`

Runs only the inference step and writes the result back into the document as an `x-typedrest` extension, for inspecting or hand-editing what the tool infers.

    typedrest-codegen pattern -f myapi.yml -o myapi-annotated.yml

| Option                    | Description                                                                        | Default                         |
| ------------------------- | ---------------------------------------------------------------------------------- | ------------------------------- |
| `-f`, `--file` (required) | The path to the Swagger or OpenAPI spec file. Use `-` to read from standard input. |                                 |
| `-o`, `--output`          | The path of the spec file to write. Use `-` to write to standard output.           | overwrites the input file       |
| `--output-version`        | The output version: `OpenApi2_0` (Swagger) or `OpenApi3_0`.                        | the version of the input        |
| `--output-format`         | The output format: `Yaml` or `Json`.                                               | based on the output file ending |

When a document already contains an `x-typedrest` extension, `generate` uses it as-is instead of re-running the inference.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
