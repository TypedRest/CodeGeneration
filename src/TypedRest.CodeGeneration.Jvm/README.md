# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for the JVM

Shared logic for generating source code for JVM-based languages. You only need this package directly if you are building your own JVM generator.

    dotnet add package TypedRest.CodeGeneration.Jvm

## Contents

The [Java](https://www.nuget.org/packages/TypedRest.CodeGeneration.Java/) and [Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/) generators target the same [TypedRest for the JVM](https://github.com/TypedRest/TypedRest-Java) artifacts, and share everything but the syntax they emit:

- **`Model/`** — the type model (`JvmIdentifier`, `JvmPackage`) and a syntax-free AST of what to emit (`JvmEndpointClass`, `JvmEndpointInterface`, `JvmDto`, `JvmEnum`, `JvmExpression`). Nothing here knows how to write itself; each language's writer renders the tree.
- **`Packages`** — the TypedRest runtime packages and the `Impl` classes generated endpoints derive from.
- **`JvmSerializer`** — the JSON serializers and the annotations each one wants on a DTO.
- The naming strategy, type mapping and endpoint builders.

`JvmSyntax.IsReservedWord` covers the *union* of Java's and Kotlin's reserved words, so a name is legal in both.

## Serializers

| Name      | Languages    | Type annotation                      | Property annotation | Artifact                                      |
| --------- | ------------ | ------------------------------------ | ------------------- | --------------------------------------------- |
| `kotlinx` | Kotlin only  | `@Serializable`                      | `@SerialName`       | `net.typedrest:typedrest`                     |
| `jackson` | Java, Kotlin |                                      | `@JsonProperty`     | `net.typedrest:typedrest-serializers-jackson` |
| `moshi`   | Java, Kotlin | `@JsonClass(generateAdapter = true)` | `@Json(name = ...)` | `net.typedrest:typedrest-serializers-moshi`   |

kotlinx.serialization generates its serializers with a Kotlin compiler plugin, so it cannot serialize a class written in Java. Kotlin defaults to `kotlinx`, Java to `jackson`.

## Related packages

- [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) is the basis of this library. It parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.
- [TypedRest.CodeGeneration.Java](https://www.nuget.org/packages/TypedRest.CodeGeneration.Java/) builds on this package.
- [TypedRest.CodeGeneration.Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/) builds on this package.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
