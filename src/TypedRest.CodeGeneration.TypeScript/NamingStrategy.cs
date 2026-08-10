using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// The default <see cref="INamingStrategy"/> for TypeScript clients.
/// </summary>
/// <param name="serviceName">The service name to use for the entry endpoint.</param>
/// <param name="endpointDirectory">The directory the endpoint modules live in, relative to the output directory.</param>
/// <param name="dtoDirectory">The directory the DTO modules live in, relative to the output directory.</param>
/// <param name="untypedFallback">The type to use for schemas that carry no usable type information.</param>
public class NamingStrategy(string serviceName, string endpointDirectory, string dtoDirectory, TsIdentifier? untypedFallback = null) : INamingStrategy
{
    /// <summary>The service name to use for the entry endpoint.</summary>
    protected readonly string ServiceName = serviceName;

    /// <summary>The directory the endpoint modules live in, relative to the output directory.</summary>
    protected readonly string EndpointDirectory = NormalizeDirectory(endpointDirectory);

    /// <summary>The directory the DTO modules live in, relative to the output directory.</summary>
    protected readonly string DtoDirectory = NormalizeDirectory(dtoDirectory);

    /// <summary>The type to use for schemas that carry no usable type information.</summary>
    protected readonly TsIdentifier UntypedFallback = untypedFallback ?? TsIdentifier.Unknown;

    /// <inheritdoc/>
    public virtual string Property(string key)
        => Words.ToCamelCase(key);

    /// <inheritdoc/>
    public virtual TsIdentifier EndpointType(string key, IEndpoint endpoint, string? prefix = null)
    {
        string prefixed = prefix is null ? "" : Words.ToPascalCase(prefix);
        string name = endpoint switch
        {
            EntryEndpoint _ => ServiceName + "Client",
            IndexerEndpoint _ => prefixed + Words.ToPascalCase(key.Depluralize()) + "CollectionEndpoint",
            _ => prefixed + Words.ToPascalCase(key) + "Endpoint"
        };

        return Generated(EndpointDirectory, name);
    }

    /// <inheritdoc/>
    public virtual TsIdentifier DtoType(string key)
    {
        var parts = key.Split(['.', '/'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
            return Generated(DtoDirectory, Words.ToPascalCase(key));

        string subDirectory = string.Join("/", parts.Take(parts.Length - 1).Select(Words.ToCamelCase));
        return Generated(Combine(DtoDirectory, subDirectory), Words.ToPascalCase(parts[parts.Length - 1]));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Every string format maps to <c>string</c>: TypedRest for TypeScript deserializes with <c>JSON.parse()</c>
    /// and a cast, so there is nothing that could turn a <c>date-time</c> into a <c>Date</c>.
    /// </remarks>
    public virtual TsIdentifier TypeFor(OpenApiSchema? schema)
    {
        var type = (schema?.Type, schema?.Format) switch
        {
            ("string", _) => TsIdentifier.String,
            ("integer", _) => TsIdentifier.Number,
            ("number", _) => TsIdentifier.Number,
            ("boolean", _) => TsIdentifier.Boolean,
            ("array", _) => TsIdentifier.ArrayOf(TypeFor(schema!.Items)),
            _ when schema?.Reference?.Id is {Length: > 0} id => DtoType(id),
            _ when schema?.AdditionalProperties is {} props => TsIdentifier.RecordOf(TsIdentifier.String, TypeFor(props)),
            _ => UntypedFallback
        };

        return schema is {Nullable: true} ? type.ToNullable() : type;
    }

    /// <summary>
    /// Builds an identifier for a type generated into <paramref name="directory"/>, one type per file.
    /// </summary>
    protected static TsIdentifier Generated(string directory, string name)
        => new(TsModule.Generated(Combine(directory, name)), name);

    /// <summary>
    /// Turns a namespace-style value such as <c>MyCompany.MyService</c> into a directory path.
    /// </summary>
    protected static string NormalizeDirectory(string? value)
        => string.IsNullOrEmpty(value)
            ? ""
            : string.Join("/", value!.Split(['.', '/'], StringSplitOptions.RemoveEmptyEntries).Select(Words.ToCamelCase));

    private static string Combine(string directory, string name)
        => directory.Length == 0 ? name : directory + "/" + name;
}
