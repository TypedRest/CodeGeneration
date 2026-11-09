# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Source Generator

Roslyn source generator that builds [TypedRest](https://typedrest.net/) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents during compilation.

    dotnet add package TypedRest.SourceGenerator
    dotnet add package TypedRest

Then reference the OpenAPI/Swagger document from your project file:

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" />
</ItemGroup>
```

The client is now part of your compilation:

```csharp
var client = new MyServiceClient(new Uri("https://example.com/api/"));
```

Add `TypedRest.Reactive` as well if your document uses polling or streaming endpoints.

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

To use System.Text.Json, add `TypedRest.SystemTextJson` to the consuming project and select it:

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" Serializer="system-text-json" />
</ItemGroup>
```

The generated entry constructor selects the matching serializer at runtime (e.g. `base(uri, serializer: new SystemTextJsonSerializer())`). If you write your own constructor (see below), pass the same serializer yourself.

## Customizing the entry point

Generated classes and interfaces are `partial`, so you can add members in your own files; generated enums cannot be partial. You can add constructor overloads to the entry endpoint, but a partial class cannot replace a constructor that has already been generated.

To replace the generated constructor or prevent callers from using it:

```xml
<ItemGroup>
  <TypedRestOpenApi Include="myapi.yml" ServiceName="MyService" GenerateEntryConstructor="false" />
</ItemGroup>
```

Write your constructor in the same namespace as the generated class. For example, with your own `MyServiceErrorHandler`:

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

If you selected `system-text-json`, also pass a `SystemTextJsonSerializer` to `base`.

## Limitations

- The generator runs inside the compiler. It references Roslyn 4.8 and depends on System.Text.Json 8 through its OpenAPI reader; the compiler host must provide compatible versions of both. Use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) if your toolchain is incompatible.
- Only local `$ref`s are resolved. Bundle multi-file specs into a single document first.

## Related packages

- [TypedRest.CodeGeneration.CSharp](https://www.nuget.org/packages/TypedRest.CodeGeneration.CSharp/) is the basis of this library. It generates C# source code for TypedRest clients.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
