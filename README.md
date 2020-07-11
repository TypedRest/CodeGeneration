# ![TypedRest](logo.svg) Code Generation

[![Build](https://github.com/TypedRest/CodeGeneration/workflows/Build/badge.svg?branch=master)](https://github.com/TypedRest/CodeGeneration/actions?query=workflow%3ABuild)  
Tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients.

## Usage

Make sure you have the [.NET Core SDK 3.1+](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-codegen

You can now use the `typedrest-codegen` command-line tool:

    typedrest-codegen generate -f myapi.yml -o myclient/

For further information take a look a the **[Documentation](https://typedrest.net/code-generation/)**.

## Custom code

If you want to generate clients for more complex APIs you may need to add custom code. You can do this by creating your own command-line tools and using these NuGet packages:

[![TypedRest.OpenApi](https://img.shields.io/nuget/v/TypedRest.OpenApi.svg?label=TypedRest.OpenApi)](https://www.nuget.org/packages/TypedRest.OpenApi/)  
Parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.

[![TypedRest.OpenApi.CSharp](https://img.shields.io/nuget/v/TypedRest.OpenApi.CSharp.svg?label=TypedRest.OpenApi.CSharp)](https://www.nuget.org/packages/TypedRest.OpenApi.CSharp/)  
Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from OpenAPI/Swagger documents.

For further information take a look a the **[API Documentation](https://code-generation.typedrest.net/)**.

References:
- http://jack.ukleja.com/code-generation-with-roslyn/
- http://roslynquoter.azurewebsites.net/
- https://stackoverflow.com/questions/32670078/how-to-generate-files-during-build-using-msbuild
