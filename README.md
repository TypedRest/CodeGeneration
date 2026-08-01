# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation

[![Build](https://github.com/TypedRest/CodeGeneration/actions/workflows/build.yml/badge.svg)](https://github.com/TypedRest/CodeGeneration/actions/workflows/build.yml)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://code-generation.typedrest.net/)  
Tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients. It currently only supports generating C# clients.

## Command-line tool

Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-codegen

You can now use the `typedrest-codegen` command-line tool:

    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-interfaces --generate-dtos

## Source generator

As an alternative to writing the generated code to disk you can generate the client during compilation:

    dotnet add package TypedRest.CodeGeneration.CSharp.SourceGenerator

Then reference the OpenAPI/Swagger document from your project file:

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" Namespace="MyCompany.MyService" />
</ItemGroup>
```

The following metadata is supported. Each of them can also be set as an MSBuild property (prefixed with `TypedRest`, e.g. `$(TypedRestServiceName)`) to provide a default for all spec files in a project.

| Metadata             | Description                                                  | Default                               |
| -------------------- | ------------------------------------------------------------ | ------------------------------------- |
| `ServiceName`        | The service name to use for the entry endpoint. Required.    |                                       |
| `Namespace`          | The C# namespace for the endpoints.                          | `$(RootNamespace)`, else service name |
| `DtoNamespace`       | The C# namespace for the DTOs.                               | the endpoint namespace                |
| `GenerateInterfaces` | Controls whether to generate interfaces for endpoints.       | `true`                                |
| `GenerateDtos`       | Controls whether to generate DTOs.                           | `true`                                |
| `LangVersion`        | The minimum C# version the generated DTOs must compile with. | the project's `$(LangVersion)`        |

Note that `GenerateInterfaces` and `GenerateDtos` default to `true` here, while the command-line tool requires them to be turned on explicitly. Generated endpoints reference the DTO types by name, so turning `GenerateDtos` off means you have to provide those types yourself.

To inspect the generated code set `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>`; the files are then written to `obj/`.

Limitations:

- The source generator runs inside the compiler, so it requires a .NET SDK that ships Roslyn 5.6 or newer (.NET SDK 10.0.302+). Use the command-line tool for older toolchains.
- Only local `$ref`s are resolved. Bundle multi-file specs into a single document first.

## Custom code

If you want to generate clients for more complex APIs you may need to add custom code. You can do this by creating your own command-line tools and using these NuGet packages:

[![TypedRest.CodeGeneration](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.svg?label=TypedRest.CodeGeneration)](https://www.nuget.org/packages/TypedRest.CodeGeneration/)  
Parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.

[![TypedRest.CodeGeneration.CSharp](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.CSharp.svg?label=TypedRest.CodeGeneration.CSharp)](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/)  
Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from OpenAPI/Swagger documents.

For further information take a look a the **[API Documentation](https://code-generation.typedrest.net/)**.

References:
- http://jack.ukleja.com/code-generation-with-roslyn/
- http://roslynquoter.azurewebsites.net/
- https://stackoverflow.com/questions/32670078/how-to-generate-files-during-build-using-msbuild

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](https://gitversion.net/).

To build run `.\build.ps1` or `./build.sh` (.NET SDK is automatically downloaded if missing using [0install](https://0install.net/)).
 
## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrains' [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
