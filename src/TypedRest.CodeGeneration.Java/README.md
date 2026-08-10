# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for Java

Generates Java source code for [TypedRest for the JVM](https://github.com/TypedRest/TypedRest-Java) clients from [OpenAPI/Swagger](https://swagger.io/resources/open-api/) documents.

    dotnet add package TypedRest.CodeGeneration.Java

Use this to build your own code generator. If you just want to generate a client for your API, use the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/) instead; it is built on this library.

> **Consuming the client from Kotlin?** Generate Kotlin instead, with [TypedRest.CodeGeneration.Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/). TypedRest for the JVM is written in Kotlin, so that is the lower-friction direction: you get `data class` DTOs, real null safety and kotlinx.serialization. This package is for projects whose own source is Java.

## Usage

```csharp
var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());
var doc = reader.Read(File.OpenRead("myapi.yml"), out _);

foreach (var file in doc.GenerateTypedRestJava(new JavaGenerationOptions("MyService")
{
    Namespace = "com.mycompany.myservice",
    GenerateDtos = true
}))
    file.WriteToDirectory("src/main/java/");
```

`GenerateTypedRestJava()` uses the endpoints described by the document's `x-typedrest` extension, or infers them from the paths using [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) if there is no such extension.

The generated code needs the TypedRest artifacts plus a serializer on the classpath:

```kotlin
dependencies {
    implementation("net.typedrest:typedrest:<version>")
    implementation("net.typedrest:typedrest-serializers-jackson:<version>")
    compileOnly("org.jspecify:jspecify:<version>")
}
```

Add `net.typedrest:typedrest-reactive` as well if the document describes any polling or streaming endpoints.

## Output

One public type per file, in a directory matching its package, as Java requires. `Namespace` is the package for the endpoints, defaulting to the service name; `DtoNamespace` is the package for the DTOs, defaulting to a `dtos` subpackage of the endpoints.

Endpoints become classes deriving from the TypedRest `Impl` classes and exposing their children as JavaBean getters over `private final` fields:

```java
client.getContacts().get("1337").getNote().read();
```

Each endpoint is built once into the field, so it keeps its response cache across calls.

With `GenerateInterfaces` each generated endpoint also gets an interface, named the way TypedRest for the JVM names its own: the interface takes the plain name and the class beside it gets the `Impl` suffix. The getters are then declared on the interface and marked `@Override` on the class.

DTOs become plain classes with public fields, a no-argument constructor and a full constructor — not `record`s, which need Java 16 and do not suit the serializers' construct-then-populate approach. Schemas with an `enum` become `enum`s.

A `$ref` inside `allOf` is flattened into the type rather than becoming a base class. Every property is still present; only the inheritance relationship is lost.

### Nullability

Properties the document does not mark as required are annotated with JSpecify's `@Nullable`. Without it Kotlin sees every generated type as a platform type and silently drops null safety across the whole DTO surface — exactly where it matters most, since an optional field really can be absent. Turn it off with `NullableAnnotations = false` if you would rather not take the dependency.

## Serializers

`Serializer` picks which annotations carry the wire names:

| Value               | Type annotation                      | Property annotation | Artifact                                      |
| ------------------- | ------------------------------------ | ------------------- | --------------------------------------------- |
| `jackson` (default) |                                      | `@JsonProperty`     | `net.typedrest:typedrest-serializers-jackson` |
| `moshi`             | `@JsonClass(generateAdapter = true)` | `@Json(name = ...)` | `net.typedrest:typedrest-serializers-moshi`   |

`kotlinx` is **not** available here: kotlinx.serialization generates its serializers with a Kotlin compiler plugin and cannot handle a class written in Java. Asking for it is an error.

Because `EntryEndpoint` defaults to kotlinx.serialization, a generated Java client always passes its serializer explicitly:

```java
public MyServiceClient(URI uri) {
    super(uri, new JacksonJsonSerializer());
}
```

## Extension points

`GenerateTypedRestJava()` takes an optional `PatternRegistry` controlling what is inferred, and an optional `BuilderRegistry` controlling what is emitted. Both live in [TypedRest.CodeGeneration.Jvm](https://www.nuget.org/packages/TypedRest.CodeGeneration.Jvm/) and are shared with the Kotlin generator, so a builder you write once affects both.

## Related packages

- [TypedRest.CodeGeneration.Jvm](https://www.nuget.org/packages/TypedRest.CodeGeneration.Jvm/) is the basis of this library and holds everything shared with the Kotlin generator.
- [TypedRest.CodeGeneration.Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/) does the same for Kotlin.
- [typedrest-codegen](https://www.nuget.org/packages/typedrest-codegen/) is a command-line tool that builds on this library and writes the generated code to disk.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
