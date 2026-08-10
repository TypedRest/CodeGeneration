# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Source Generator

Roslyn source generator that builds [TypedRest](https://typedrest.net/) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents during compilation.

    dotnet add package TypedRest.SourceGenerator

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

| Metadata                   | Description                                                                 | Default                               |
| -------------------------- | --------------------------------------------------------------------------- | ------------------------------------- |
| `ServiceName`              | The service name to use for the entry endpoint. Required.                   |                                       |
| `Namespace`                | The C# namespace for the endpoints.                                         | `$(RootNamespace)`, else service name |
| `DtoNamespace`             | The C# namespace for the DTOs.                                              | the endpoint namespace                |
| `GenerateInterfaces`       | Controls whether to generate interfaces for endpoints.                      | `true`                                |
| `GenerateDtos`             | Controls whether to generate DTOs.                                          | `true`                                |
| `GenerateEntryConstructor` | Controls whether the entry endpoint gets a constructor taking the base URI. | `true`                                |
| `Serializer`               | The JSON serializer the generated DTOs are annotated for. See below.        | `newtonsoft`                          |

Note that `GenerateInterfaces` and `GenerateDtos` default to `true` here, while the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) requires them to be turned on explicitly. Generated endpoints reference the DTO types by name, so turning `GenerateDtos` off means you have to provide those types yourself.

## Choosing a serializer

`Serializer` picks which attributes carry the wire names on the generated DTOs:

| Value              | Property attribute   | Enum value attribute         | Runtime package                                                                        |
| ------------------ | -------------------- | ---------------------------- | -------------------------------------------------------------------------------------- |
| `newtonsoft`       | `[JsonProperty]`     | `[EnumMember]`               | [`TypedRest`](https://www.nuget.org/packages/TypedRest/)                               |
| `system-text-json` | `[JsonPropertyName]` | `[JsonStringEnumMemberName]` | [`TypedRest.SystemTextJson`](https://www.nuget.org/packages/TypedRest.SystemTextJson/) |

```xml
<PropertyGroup>
  <TypedRestSerializer>system-text-json</TypedRestSerializer>
</PropertyGroup>
```

This has to match the serializer the endpoint is configured with at runtime. The two read entirely different attributes, so a DTO annotated for one silently falls back to its C# member names under the other, changing the wire format without any error.

`[JsonStringEnumMemberName]` requires .NET 9 or later. It is what `JsonStringEnumConverter` reads; System.Text.Json ignores `[EnumMember]` entirely.

## Customizing the entry point

Every generated type is `partial`, so you can add members in your own files. The entry endpoint is the one place where that is not enough on its own: its constructor is what passes the base URI, the error handler and any default headers to `EntryEndpoint`, and a partial class cannot change a constructor that has already been generated.

Set `GenerateEntryConstructor="false"` to have the generator emit the class and its base type but no constructor, then write your own:

```xml
<TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" GenerateEntryConstructor="false" />
```

```csharp
public partial class MyServiceClient
{
    public MyServiceClient(Uri uri, string token)
        : base(uri, errorHandler: new MyServiceErrorHandler())
    {
        HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }
}
```

To inspect the generated code set `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>`; the files are then written to `obj/`.

## Diagnostics

| ID        | Severity | Meaning                                                                                   |
| --------- | -------- | ----------------------------------------------------------------------------------------- |
| `TRCG001` | Error    | A `TypedRestOpenApi` item has no `ServiceName` and no `$(TypedRestServiceName)` fallback. |
| `TRCG002` | Error    | The OpenAPI document contains an error.                                                   |
| `TRCG003` | Warning  | The OpenAPI document contains something questionable.                                     |
| `TRCG004` | Error    | Code generation failed for a document.                                                    |
| `TRCG005` | Warning  | The content of a spec file could not be read.                                             |
| `TRCG006` | Error    | A `TypedRestOpenApi` item specifies an unknown `Serializer`.                              |

## Limitations

- The generator runs inside the compiler, so it requires a .NET SDK that ships Roslyn 5.6 or newer (.NET SDK 10.0.302+). Use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) for older toolchains.
- Only local `$ref`s are resolved. Bundle multi-file specs into a single document first.

## Related packages

- [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/) is the basis of this library. It generates C# source code for TypedRest clients.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
