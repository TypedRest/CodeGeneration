using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// The default <see cref="INamingStrategy"/> for JVM clients.
/// </summary>
/// <param name="serviceName">The service name to use for the entry endpoint.</param>
/// <param name="endpointPackage">The package the endpoints are generated into.</param>
/// <param name="dtoPackage">The package the DTOs are generated into.</param>
/// <param name="untypedFallback">The type to use for schemas that carry no usable type information.</param>
public class NamingStrategy(string serviceName, string endpointPackage, string dtoPackage, JvmIdentifier? untypedFallback = null) : INamingStrategy
{
    /// <summary>The service name to use for the entry endpoint.</summary>
    protected readonly string ServiceName = serviceName;

    /// <summary>The package the endpoints are generated into.</summary>
    protected readonly JvmPackage EndpointPackage = JvmPackage.ForGenerated(JvmPackage.Sanitize(endpointPackage));

    /// <summary>The package the DTOs are generated into.</summary>
    protected readonly JvmPackage DtoPackage = JvmPackage.ForGenerated(JvmPackage.Sanitize(dtoPackage));

    /// <summary>The type to use for schemas that carry no usable type information.</summary>
    protected readonly JvmIdentifier UntypedFallback = untypedFallback ?? JvmIdentifier.Object;

    /// <inheritdoc/>
    public virtual string Property(string key)
        => JvmSyntax.Identifier(Words.ToCamelCase(key));

    /// <inheritdoc/>
    public virtual JvmIdentifier EndpointType(string key, IEndpoint endpoint, string? prefix = null)
    {
        string prefixed = prefix is null ? "" : Words.ToPascalCase(prefix);
        string name = endpoint switch
        {
            EntryEndpoint _ => ServiceName + "Client",
            IndexerEndpoint _ => prefixed + Words.ToPascalCase(key.Depluralize()) + "CollectionEndpoint",
            _ => prefixed + Words.ToPascalCase(key) + "Endpoint"
        };

        return new JvmIdentifier(EndpointPackage, JvmSyntax.Identifier(name));
    }

    /// <inheritdoc/>
    public virtual JvmIdentifier DtoType(string key)
    {
        var parts = key.Split(['.', '/'], StringSplitOptions.RemoveEmptyEntries);
        string name = parts.Length == 0 ? key : string.Concat(parts.Select(Words.ToPascalCase));

        return new JvmIdentifier(DtoPackage, JvmSyntax.Identifier(Words.ToPascalCase(name)));
    }

    /// <inheritdoc/>
    public virtual JvmIdentifier TypeFor(OpenApiSchema? schema)
    {
        var type = (schema?.Type, schema?.Format) switch
        {
            ("string", "date-time") => JvmIdentifier.OffsetDateTime,
            ("string", "date") => JvmIdentifier.LocalDate,
            ("string", "uuid") => JvmIdentifier.Uuid,
            ("string", "binary") => JvmIdentifier.InputStream,
            ("string", _) => JvmIdentifier.String,
            ("integer", "int64") => JvmIdentifier.Long,
            ("integer", _) => JvmIdentifier.Int,
            ("number", _) => JvmIdentifier.Double,
            ("boolean", _) => JvmIdentifier.Boolean,
            ("array", _) => JvmIdentifier.ListOf(TypeFor(schema!.Items)),
            _ when schema?.Reference?.Id is {Length: > 0} id => DtoType(id),
            _ when schema?.AdditionalProperties is {} props => JvmIdentifier.MapOf(TypeFor(props)),
            _ => UntypedFallback
        };

        return schema is {Nullable: true} ? type.ToNullable() : type;
    }
}
