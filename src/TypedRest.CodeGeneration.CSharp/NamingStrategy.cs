using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.CSharp;

public class NamingStrategy(string serviceName, string endpointNamespace, string dtoNamespace) : INamingStrategy
{
    protected readonly string ServiceName = serviceName;
    protected readonly string EndpointNamespace = endpointNamespace;
    protected readonly string DtoNamespace = dtoNamespace;

    public virtual string Property(string key)
        => Normalize(key);

    public virtual CSharpIdentifier EndpointType(string key, IEndpoint endpoint, string? prefix = null)
    {
        string prefixed = prefix is null ? "" : Normalize(prefix);
        return new(
            EndpointNamespace,
            endpoint switch
            {
                EntryEndpoint _ => ServiceName + "Client",
                IndexerEndpoint _ => prefixed + Normalize(key.Depluralize()) + "CollectionEndpoint",
                _ => prefixed + Normalize(key) + "Endpoint"
            });
    }

    public virtual CSharpIdentifier DtoType(string key)
    {
        var parts = key.Split(['.', '/'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
            return new(DtoNamespace, Normalize(key));

        string subNamespace = string.Join(".", parts.Take(parts.Length - 1).Select(Normalize));
        return new($"{DtoNamespace}.{subNamespace}", Normalize(parts[parts.Length - 1]));
    }

    public virtual CSharpIdentifier TypeFor(OpenApiSchema? schema, bool nullableReferenceTypes = true)
    {
        var type = (schema?.Type, schema?.Format) switch
        {
            ("string", "uri") => CSharpIdentifier.Uri,
            ("string", "uuid" or "guid") => Guid,
            ("string", "date-time") => DateTimeOffset,
            ("string", "date") => DateTime,
            ("string", "time" or "duration") => TimeSpan,
            ("string", _) => CSharpIdentifier.String,
            ("integer", "int64") => CSharpIdentifier.Long,
            ("integer", _) => CSharpIdentifier.Int,
            ("number", "float") => CSharpIdentifier.Float,
            ("number", "decimal") => Decimal,
            ("number", _) => CSharpIdentifier.Double,
            ("boolean", _) => CSharpIdentifier.Bool,
            ("array", _) => CSharpIdentifier.ListOf(TypeFor(schema.Items, nullableReferenceTypes)),
            _ when schema?.Reference?.Id is {Length: > 0} id => DtoType(id),
            _ when schema?.AdditionalProperties is {} props => CSharpIdentifier.DictionaryOf(CSharpIdentifier.String, TypeFor(props, nullableReferenceTypes)),
            _ => new CSharpIdentifier("Newtonsoft.Json.Linq", "JObject")
        };

        bool isValueType = schema?.Type is "integer" or "number" or "boolean" || schema.HasValueTypeFormat();
        return schema is {Nullable: true} && (nullableReferenceTypes || isValueType)
            ? type.ToNullable()
            : type;
    }

    private static CSharpIdentifier Guid => new("System", "Guid");
    private static CSharpIdentifier DateTime => new("System", "DateTime");
    private static CSharpIdentifier DateTimeOffset => new("System", "DateTimeOffset");
    private static CSharpIdentifier TimeSpan => new("System", "TimeSpan");
    private static CSharpIdentifier Decimal => new(null, "decimal");

    protected virtual string Normalize(string key)
        => Words.ToPascalCase(key);
}
