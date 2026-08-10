# ![TypedRest](https://raw.githubusercontent.com/TypedRest/TypedRest-DotNet/master/logo.svg) Code Generation for the JVM

Everything the [Java](https://www.nuget.org/packages/TypedRest.CodeGeneration.Java/) and [Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/) generators have in common. You only need this package directly if you are building your own JVM generator; to generate a client, use one of those two or the [command-line tool](https://www.nuget.org/packages/typedrest-codegen/).

    dotnet add package TypedRest.CodeGeneration.Jvm

## Why a shared core

[TypedRest for the JVM](https://github.com/TypedRest/TypedRest-Java) is written in Kotlin and published as a single set of Maven artifacts that both languages consume. A generated Java client and a generated Kotlin client therefore derive from the *same* runtime types, and every decision about which type an endpoint maps to is identical between them:

```kotlin
val contacts: GenericCollectionEndpointImpl<Contact, ContactElementEndpoint> =
    GenericCollectionEndpointImpl(this, "contacts", Contact::class.java) { r, u -> ContactElementEndpoint(r, u) }
```
```java
public final GenericCollectionEndpointImpl<Contact, ContactElementEndpoint> contacts =
    new GenericCollectionEndpointImpl<>(this, "contacts", Contact.class, ContactElementEndpoint::new);
```

Only the syntax differs. So this package holds the parts that do not:

- **`Model/`** — the type model (`JvmIdentifier`, `JvmPackage`) and a syntax-free AST of what to emit (`JvmEndpointClass`, `JvmDto`, `JvmEnum`, `JvmExpression`). Nothing here knows how to write itself; each language's writer renders the tree.
- **`Packages`** — the TypedRest runtime packages and the `Impl` classes generated endpoints derive from.
- **`JvmSerializer`** — the JSON serializers and the annotations each one wants on a DTO.
- The naming strategy, type mapping and endpoint builders.

`JvmSyntax.IsReservedWord` deliberately uses the *union* of Java's and Kotlin's reserved words. The two generators share a naming strategy, so a name legal in only one of them would make the same document produce a compiling client in one language and a broken one in the other.

## Serializers

| Name      | Languages    | Type annotation                     | Property annotation | Artifact                                    |
| --------- | ------------ | ----------------------------------- | ------------------- | ------------------------------------------- |
| `kotlinx` | Kotlin only  | `@Serializable`                     | `@SerialName`       | `net.typedrest:typedrest`                   |
| `jackson` | Java, Kotlin |                                     | `@JsonProperty`     | `net.typedrest:typedrest-serializers-jackson` |
| `moshi`   | Java, Kotlin | `@JsonClass(generateAdapter = true)` | `@Json(name = ...)` | `net.typedrest:typedrest-serializers-moshi`   |

kotlinx.serialization generates its serializers with a Kotlin compiler plugin, so it cannot serialize a class written in Java. This is why the two generators default differently: Kotlin defaults to `kotlinx`, matching the default of `EntryEndpoint` itself, while Java defaults to `jackson` and passes it to the entry endpoint explicitly.

## Related packages

- [TypedRest.CodeGeneration](https://www.nuget.org/packages/TypedRest.CodeGeneration/) is the basis of this library. It parses OpenAPI/Swagger documents and infers TypedRest Endpoints from patterns.
- [TypedRest.CodeGeneration.Kotlin](https://www.nuget.org/packages/TypedRest.CodeGeneration.Kotlin/) and [TypedRest.CodeGeneration.Java](https://www.nuget.org/packages/TypedRest.CodeGeneration.Java/) build on this one.

## Links

- [Code generation documentation](https://typedrest.net/code-generation/)
- [API documentation](https://code-generation.typedrest.net/)
