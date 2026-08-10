# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation

[![Build](https://github.com/TypedRest/CodeGeneration/actions/workflows/build.yml/badge.svg)](https://github.com/TypedRest/CodeGeneration/actions/workflows/build.yml)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://code-generation.typedrest.net/)  
Tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients. It can generate C# and TypeScript clients.

Generate a C# client during compilation:

    dotnet add package TypedRest.CodeGeneration.CSharp.SourceGenerator

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" Namespace="MyCompany.MyService" />
</ItemGroup>
```

Or write it to disk with the command-line tool:

    dotnet tool install -g typedrest-codegen
    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-interfaces --generate-dtos

The same tool generates TypeScript clients for [TypedRest for TypeScript](https://github.com/TypedRest/TypedRest-TypeScript):

    typedrest-codegen generate -l typescript -f myapi.yml -o src/myclient/ -s MyService --generate-dtos

Read the **[Code generation documentation](https://typedrest.net/code-generation/)** for how the inference works, how to configure both tools and what to do when the inferred client is not what you want.

## NuGet packages

[![TypedRest.CodeGeneration.CSharp.SourceGenerator](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.CSharp.SourceGenerator.svg?label=TypedRest.CodeGeneration.CSharp.SourceGenerator)](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp.SourceGenerator/)  
Roslyn source generator that builds clients during compilation.

[![typedrest-codegen](https://img.shields.io/nuget/v/typedrest-codegen.svg?label=typedrest-codegen)](https://www.nuget.org/packages/typedrest-codegen/)  
Command-line tool that writes the generated code to disk.

[![TypedRest.CodeGeneration](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.svg?label=TypedRest.CodeGeneration)](https://www.nuget.org/packages/TypedRest.CodeGeneration/)  
Parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.

[![TypedRest.CodeGeneration.CSharp](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.CSharp.svg?label=TypedRest.CodeGeneration.CSharp)](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/)  
Generates C# source code for TypedRest .NET clients from OpenAPI/Swagger documents.

[![TypedRest.CodeGeneration.TypeScript](https://img.shields.io/nuget/v/TypedRest.CodeGeneration.TypeScript.svg?label=TypedRest.CodeGeneration.TypeScript)](https://www.nuget.org/packages/TypedRest.CodeGeneration.TypeScript/)  
Generates TypeScript source code for TypedRest clients from OpenAPI/Swagger documents.

You can also [build your own generator](https://typedrest.net/code-generation/custom-code/) for more complex APIs. For the relevant types and methods take a look at the **[API documentation](https://code-generation.typedrest.net/)**.

## Building

The source code is in [`src/`](src/), config for building the API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`. The source code does not contain version numbers. Instead the version is determined during CI using [GitVersion](https://gitversion.net/).

To build run `.\build.ps1` or `./build.sh` (.NET SDK is automatically downloaded if missing using [0install](https://0install.net/)).

## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrains' [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.
