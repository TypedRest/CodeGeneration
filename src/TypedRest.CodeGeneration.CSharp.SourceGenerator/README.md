# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation Source Generator

Roslyn source generator that builds [TypedRest](https://typedrest.net/) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents during compilation.

    dotnet add package TypedRest.CodeGeneration.CSharp.SourceGenerator

Then reference the OpenAPI/Swagger document from your project file:

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" Namespace="MyCompany.MyService" />
</ItemGroup>
```

The client is now part of your compilation:

```csharp
var client = new MyServiceClient(new Uri("https://example.com/api/"));
```

For a walkthrough see the **[usage guide](https://typedrest.net/code-generation/source-generator/)**. This page is the reference for the configuration knobs and diagnostics.

## Configuration

Each `TypedRestOpenApi` item supports the following metadata. Each can also be set as an MSBuild property (prefixed with `TypedRest`, e.g. `$(TypedRestServiceName)`) to provide a default for all spec files in a project.

| Metadata             | Description                                                  | Default                               |
| -------------------- | ------------------------------------------------------------ | ------------------------------------- |
| `ServiceName`        | The service name to use for the entry endpoint. Required.    |                                       |
| `Namespace`          | The C# namespace for the endpoints.                          | `$(RootNamespace)`, else service name |
| `DtoNamespace`       | The C# namespace for the DTOs.                               | the endpoint namespace                |
| `GenerateInterfaces` | Controls whether to generate interfaces for endpoints.       | `true`                                |
| `GenerateDtos`       | Controls whether to generate DTOs.                           | `true`                                |
| `LangVersion`        | The minimum C# version the generated DTOs must compile with. | the project's `$(LangVersion)`        |

Note that `GenerateInterfaces` and `GenerateDtos` default to `true` here, while the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) requires them to be turned on explicitly. Generated endpoints reference the DTO types by name, so turning `GenerateDtos` off means you have to provide those types yourself.

To inspect the generated code set `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>`; the files are then written to `obj/`.

## Diagnostics

| ID        | Severity | Meaning                                                                            |
| --------- | -------- | ---------------------------------------------------------------------------------- |
| `TRCG001` | Error    | A `TypedRestOpenApi` item has no `ServiceName` and no `$(TypedRestServiceName)` fallback. |
| `TRCG002` | Error    | The OpenAPI document contains an error.                                            |
| `TRCG003` | Warning  | The OpenAPI document contains something questionable.                              |
| `TRCG004` | Warning  | `LangVersion` is not a valid value; the project's language version is used instead. |
| `TRCG005` | Error    | Code generation failed for a document.                                             |
| `TRCG006` | Warning  | The content of a spec file could not be read.                                      |

## Limitations

- The generator runs inside the compiler, so it requires a .NET SDK that ships Roslyn 5.6 or newer (.NET SDK 10.0.302+). Use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) for older toolchains.
- Only local `$ref`s are resolved. Bundle multi-file specs into a single document first.

## Related packages

- [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/) is the basis of this library. It generates C# source code for TypedRest clients.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
