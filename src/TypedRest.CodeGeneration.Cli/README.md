# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation CLI

Command-line tool that automatically infers [TypedRest Endpoints](https://typedrest.net/endpoints/) from patterns in [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents and generates source code for TypedRest clients.

Make sure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed and run:

    dotnet tool install -g typedrest-codegen

If you build your client with the .NET SDK, consider the [source generator](https://www.nuget.org/packages/TypedRest.SourceGenerator/) instead. It runs the same generator during compilation, without writing files to your source tree.

For a walkthrough see the **[usage guide](https://typedrest.net/code-generation/cli/)**.

## `generate`

Generates a TypedRest client.

    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-interfaces --generate-dtos

| Option                            | Description                                                                                                                            | Default          |
| --------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- | ---------------- |
| `-f`, `--file` (required)         | The path to the Swagger or OpenAPI spec file. Use `-` to read from standard input.                                                     |                  |
| `-o`, `--output` (required)       | The directory to write the generated source code to.                                                                                   |                  |
| `-s`, `--service-name` (required) | The service name to use for the entry endpoint.                                                                                        |                  |
| `-l`, `--language`                | The language to generate: `csharp`, `typescript`, `kotlin` or `java`.                                                                  | `csharp`         |
| `-n`, `--namespace`               | The namespace (C#), package (Kotlin/Java) or directory (TypeScript) for the endpoints.                                                 | the service name |
| `--dto-namespace`                 | The same for the DTOs.                                                                                                                 | see below        |
| `--generate-interfaces`           | Also generate interfaces for the endpoints. **C# only.**                                                                               | off              |
| `--generate-dtos`                 | Also generate DTOs for the schemas in the document.                                                                                    | off              |
| `--generate-entry-constructor`    | Give the entry endpoint a constructor taking the base URI. Pass `false` to write your own. **Not for TypeScript.**                     | on               |
| `--lang-version`                  | The minimum C# version the generated code must compile with, using the same values as the MSBuild `LangVersion` property. **C# only.** | `latest`         |
| `--serializer`                    | The JSON serializer the generated DTOs are annotated for. See below.                                                                   | per language     |

### C#

The generated code derives from the [`TypedRest`](https://www.nuget.org/packages/TypedRest/) package, so run `dotnet add package TypedRest` in the consuming project.

Unlike the [source generator](https://www.nuget.org/packages/TypedRest.SourceGenerator/), interfaces and DTOs are opt-in here. Generated endpoints reference the DTO types by name, so without `--generate-dtos` you have to provide those types yourself.

Use `--generate-entry-constructor false` when the entry endpoint needs a custom error handler or default headers. The class and its base type are still generated, but the constructor is left for you to write in a partial class.

`--serializer` picks which attributes carry the wire names on the generated DTOs:

| Value              | Property attribute   | Enum value attribute         | Runtime package                                                                        |
| ------------------ | -------------------- | ---------------------------- | -------------------------------------------------------------------------------------- |
| `newtonsoft`       | `[JsonProperty]`     | `[EnumMember]`               | [`TypedRest`](https://www.nuget.org/packages/TypedRest/)                               |
| `system-text-json` | `[JsonPropertyName]` | `[JsonStringEnumMemberName]` | [`TypedRest.SystemTextJson`](https://www.nuget.org/packages/TypedRest.SystemTextJson/) |

    typedrest-codegen generate -f myapi.yml -o myclient/ -s MyService --generate-dtos --serializer system-text-json

This has to match the serializer the endpoint is configured with at runtime. The two read entirely different attributes, so a DTO annotated for one silently falls back to its C# member names under the other, changing the wire format without any error.

`[JsonStringEnumMemberName]` requires .NET 9 or later. It is what `JsonStringEnumConverter` reads; System.Text.Json ignores `[EnumMember]` entirely.

### TypeScript

    typedrest-codegen generate -l typescript -f myapi.yml -o src/myclient/ -s MyService --generate-dtos

The generated code imports from the [`typedrest`](https://www.npmjs.com/package/typedrest) package, so run `npm install typedrest` in the consuming project.

Each generated type gets its own file, plus an `index.ts` re-exporting all of them. Here `--namespace` and `--dto-namespace` are directories relative to `--output` rather than namespaces, defaulting to the output directory itself and to `dtos`. Dotted values such as `MyCompany.MyService` become nested directories.

Endpoints become classes deriving from the TypedRest endpoint types and exposing their children as getters. DTOs become interfaces whose properties keep the exact name used on the wire, because TypedRest for TypeScript deserializes with `JSON.parse()` and a cast and so has no way to map a property to a differently named field. Schemas with an `enum` become literal union type aliases.

`--serializer` has no effect here, for the same reason: there is no serializer to choose and nothing to annotate.

### Kotlin

    typedrest-codegen generate -l kotlin -f myapi.yml -o src/main/kotlin/ -s MyService -n com.mycompany.myservice --generate-dtos

The generated code derives from [TypedRest for the JVM](https://github.com/TypedRest/TypedRest-Java), so add `net.typedrest:typedrest` to the consuming project — plus `net.typedrest:typedrest-reactive` if the document describes any polling or streaming endpoints.

One file per type, in a directory matching its package, so `--output` is the source root (`src/main/kotlin/`) rather than the package directory. `--namespace` is the package for the endpoints and `--dto-namespace` the one for the DTOs, defaulting to a `dtos` subpackage of the endpoints.

Endpoints become `open class`es deriving from the TypedRest `Impl` classes and exposing their children as `val`s. DTOs become `data class`es, and schemas with an `enum` become `enum class`es. Optional properties are nullable and default to `null`; required ones get no default, so a missing value is a compile error.

`--serializer` picks `kotlinx` (default), `jackson` or `moshi`. kotlinx.serialization is what `EntryEndpoint` itself defaults to, so a client generated for it passes no serializer at all; the others are passed explicitly.

Generating DTOs for `kotlinx` needs both the `kotlin("plugin.serialization")` Gradle plugin **and** an explicit `org.jetbrains.kotlinx:kotlinx-serialization-json` dependency: TypedRest depends on it only as `implementation`, so it does not reach your compile classpath, and the plugin adds the compiler plugin but no dependency.

### Java

    typedrest-codegen generate -l java -f myapi.yml -o src/main/java/ -s MyService -n com.mycompany.myservice --generate-dtos

Prefer the Kotlin generator if you can: TypedRest for the JVM is written in Kotlin, so that is the lower-friction direction. Use this one when your own source is Java.

The layout matches the Kotlin generator's. Endpoints expose their children as `public final` fields rather than getters, because a getter recomputing the endpoint on every call would hand out a new instance each time and throw away the response cache. DTOs become plain classes with public fields, a no-argument constructor and a full one.

Properties the document does not require are annotated with JSpecify's `@Nullable`, so that Kotlin consumers get real null safety instead of platform types. Add `org.jspecify:jspecify` to the consuming project, or drop the annotations by generating Kotlin instead.

`--serializer` picks `jackson` (default) or `moshi`. `kotlinx` is rejected here: kotlinx.serialization generates its serializers with a Kotlin compiler plugin and cannot handle a class written in Java, so a client generated for it would compile and then fail to deserialize anything.

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
