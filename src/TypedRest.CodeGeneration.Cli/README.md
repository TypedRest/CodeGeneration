# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation CLI

Command-line tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients.

Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-codegen

If you build your client with the .NET SDK, consider the [source generator](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp.SourceGenerator/) instead. It runs the same generator during compilation, without writing files to your source tree.

For a walkthrough see the **[usage guide](https://typedrest.net/code-generation/cli/)**.

## `generate`

Generates a TypedRest client.

    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-interfaces --generate-dtos

| Option                            | Description                                                                                                               | Default                |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------- | ---------------------- |
| `-f`, `--file` (required)         | The path to the Swagger or OpenAPI spec file. Use `-` to read from standard input.                                        |                        |
| `-o`, `--output` (required)       | The directory to write the generated source code to.                                                                      |                        |
| `-s`, `--service-name` (required) | The service name to use for the entry endpoint.                                                                           |                        |
| `-n`, `--namespace`               | The C# namespace for the endpoints.                                                                                       | the service name       |
| `--dto-namespace`                 | The C# namespace for the DTOs.                                                                                            | the endpoint namespace |
| `--generate-interfaces`           | Also generate interfaces for the endpoints.                                                                               | off                    |
| `--generate-dtos`                 | Also generate DTOs for the schemas in the document.                                                                       | off                    |
| `--lang-version`                  | The minimum C# version the generated DTOs must compile with, using the same values as the MSBuild `LangVersion` property. | `latest`               |

Unlike the [source generator](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp.SourceGenerator/), interfaces and DTOs are opt-in here. Generated endpoints reference the DTO types by name, so without `--generate-dtos` you have to provide those types yourself.

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
