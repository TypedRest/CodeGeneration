# TypedRest OpenAPI

[![Build status](https://img.shields.io/appveyor/ci/TypedRest/typedrest-openapi.svg)](https://ci.appveyor.com/project/TypedRest/typedrest-openapi)  
Tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients.

## Usage

Make sure you have the [.NET Core SDK](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-openapi

You can now use the `typedrest-openapi` command-line tool:

    typedrest-openapi --input myapi.yml --output myclient/

For further information take a look a the **[TypedRest OpenAPI Documentation](https://typedrest.net/openapi/)**.

## Custom code

If you want to generate clients for more complex APIs you may need to add custom code. You can do this by creating your own command-line tools and using these NuGet packages:

[![TypedRest.OpenApi](https://img.shields.io/nuget/v/TypedRest.OpenApi.svg?label=TypedRest.OpenApi)](https://www.nuget.org/packages/TypedRest.OpenApi/)  
Parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.

[![TypedRest.OpenApi.CSharp](https://img.shields.io/nuget/v/TypedRest.OpenApi.CSharp.svg?label=TypedRest.OpenApi.CSharp)](https://www.nuget.org/packages/TypedRest.OpenApi.CSharp/)  
Generates C# source code for [TypedRest .NET](https://github.com/TypedRest/TypedRest-DotNet) clients from OpenAPI/Swagger documents.

For further information take a look a the **[API Documentation](https://openapi.typedrest.net/)**.
