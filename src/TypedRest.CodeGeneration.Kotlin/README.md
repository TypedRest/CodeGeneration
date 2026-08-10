# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for Kotlin

Generates Kotlin source code for [TypedRest for the JVM](https://github.com/TypedRest/TypedRest-Java) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.

    dotnet add package TypedRest.CodeGeneration.Kotlin

Use this to build your own code generator. If you just want to generate a client for your API, use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) instead; it is built on this library.

## Usage

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

foreach (var file in doc.GenerateTypedRestKotlin(new KotlinGenerationOptions("MyService")
{
    Namespace = "com.mycompany.myservice",
    GenerateDtos = true
}))
    file.WriteToDirectory("src/main/kotlin/");
```

`GenerateTypedRestKotlin()` uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths using [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) if there is no such extension.

The generated code needs the TypedRest artifacts on the classpath:

```kotlin
dependencies {
    implementation("net.typedrest:typedrest:<version>")

    // Only when generating DTOs for the default kotlinx serializer
    implementation("org.jetbrains.kotlinx:kotlinx-serialization-json:<version>")
}
```

Add `net.typedrest:typedrest-reactive` as well if the document describes any polling or streaming endpoints.

The explicit `kotlinx-serialization-json` is easy to miss: TypedRest depends on it only as `implementation`, so it does not reach a consumer's compile classpath, and the `kotlin("plugin.serialization")` plugin adds the compiler plugin but no dependency. Without it the `@Serializable` and `@SerialName` annotations on the generated DTOs do not resolve.

## Output

One file per generated type, in a directory matching its package. `Namespace` is the package for the endpoints, defaulting to the service name; `DtoNamespace` is the package for the DTOs, defaulting to a `dtos` subpackage of the endpoints.

Endpoints become `open class`es deriving from the TypedRest `Impl` classes and exposing their children as `val`s. They are `open` so that you can derive from them to add members of your own.

DTOs become `data class`es, and schemas with an `enum` become `enum class`es. A property the document does not mark as required is nullable and defaults to `null`, so a DTO can be built without naming every optional field; a required one deliberately gets no default, making a missing value a compile error.

A `$ref` inside `allOf` is flattened into the type rather than becoming a base class, because a Kotlin `data class` is final and cannot be derived from. Every property is still present; only the inheritance relationship is lost.

## Serializers

`Serializer` picks which annotations carry the wire names:

| Value               | Type annotation                      | Property annotation | Artifact                                      |
| ------------------- | ------------------------------------ | ------------------- | --------------------------------------------- |
| `kotlinx` (default) | `@Serializable`                      | `@SerialName`       | `net.typedrest:typedrest`                     |
| `jackson`           |                                      | `@JsonProperty`     | `net.typedrest:typedrest-serializers-jackson` |
| `moshi`             | `@JsonClass(generateAdapter = true)` | `@Json(name = ...)` | `net.typedrest:typedrest-serializers-moshi`   |

kotlinx.serialization is the default of `EntryEndpoint` itself, so a client generated for it passes no serializer at all. The others are passed explicitly in the generated entry endpoint constructor.

Generating for `kotlinx` requires the `kotlin-serialization` Gradle plugin in the consuming project — the `@Serializable` annotation does nothing without the compiler plugin that acts on it:

```kotlin
plugins {
    kotlin("plugin.serialization") version "<version>"
}
```

## Extension points

`GenerateTypedRestKotlin()` takes an optional `PatternRegistry` controlling what is inferred, and an optional `BuilderRegistry` controlling what is emitted:

```csharp
var files = doc.GenerateTypedRestKotlin(options, log, patterns, builders);
```

Both registries live in [TypedRest.CodeGeneration.Jvm](https://www.nuget.org/packages/TypedRest.CodeGeneration.Jvm/) and are shared with the Java generator, because both languages target the same runtime types. Implement `IBuilder<TEndpoint>` to change the code emitted for an endpoint kind, or derive from `NamingStrategy` to change how types and properties are named.

## Related packages

- [TypedRest.CodeGeneration.Jvm](https://www.nuget.org/packages/TypedRest.CodeGeneration.Jvm/) is the basis of this library and holds everything shared with the Java generator.
- [TypedRest.CodeGeneration.Java](https://www.nuget.org/packages/TypedRest.CodeGeneration.Java/) does the same for Java.
- [typedrest-codegen](https://www.nuget.org/packages/typedrest-codegen/) is a command-line tool that builds on this library and writes the generated code to disk.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
