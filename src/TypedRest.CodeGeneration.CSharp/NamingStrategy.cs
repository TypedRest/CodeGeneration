using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp;

public class NamingStrategy : INamingStrategy
{
    protected readonly string ServiceName;
    protected readonly string EndpointNamespace;
    protected readonly string DtoNamespace;

    public NamingStrategy(string serviceName, string endpointNamespace, string dtoNamespace)
    {
        ServiceName = serviceName;
        EndpointNamespace = endpointNamespace;
        DtoNamespace = dtoNamespace;
    }

    public virtual string Property(string key)
        => Normalize(key);

    public virtual CSharpIdentifier EndpointType(string key, IEndpoint endpoint)
        => new(
            EndpointNamespace,
            endpoint switch
            {
                EntryEndpoint _ => ServiceName + "Client",
                // TODO: Avoid name clashes when the same key is used in different places
                IndexerEndpoint _ when key.EndsWith("s") => Normalize(key.Substring(0, key.Length - 1)) + "CollectionEndpoint",
                _ => Normalize(key) + "Endpoint"
            });

    public virtual CSharpIdentifier DtoType(string key)
        // TODO: Extract namespace from key
        => new(
            DtoNamespace,
            Normalize(key));

    public virtual CSharpIdentifier TypeFor(OpenApiSchema? schema)
        => (schema?.Type, schema?.Format) switch
        {
            ("string", "uri") => CSharpIdentifier.Uri,
            ("string", _) => CSharpIdentifier.String,
            ("integer", "int64") => CSharpIdentifier.Long.ToNullable(),
            ("integer", _) => CSharpIdentifier.Int.ToNullable(),
            ("number", "float") => CSharpIdentifier.Float.ToNullable(),
            ("number", _) => CSharpIdentifier.Double.ToNullable(),
            ("boolean", _) => CSharpIdentifier.Bool.ToNullable(),
            ("array", _) => CSharpIdentifier.ListOf(TypeFor(schema.Items)),
            _ when schema?.Reference?.Id is {Length: > 0} id => DtoType(id),
            _ when schema?.AdditionalProperties is {} props => CSharpIdentifier.DictionaryOf(CSharpIdentifier.String, TypeFor(props)),
            _ => new CSharpIdentifier("Newtonsoft.Json.Linq", "JObject")
        };

    private static readonly char[] _separators = {' ', '-', '_', '.'};

    protected virtual string Normalize(string key)
    {
        key = key.Replace("$", "");
        return key switch
        {
            "" => "",
            null => "",
            // kebap-case or snake_case
            {} when _separators.Any(key.Contains) => string.Concat(
                key.Split(_separators, StringSplitOptions.RemoveEmptyEntries)
                   .Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower())),
            // CamelCase
            _ => (key.Substring(0, 1).ToUpper() + key.Substring(1))
        };
    }
}
