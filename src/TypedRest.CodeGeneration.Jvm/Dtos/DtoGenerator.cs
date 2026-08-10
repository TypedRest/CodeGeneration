using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm.Dtos;

/// <summary>
/// Generates types for the schemas in an OpenAPI/Swagger document.
/// </summary>
/// <param name="naming">Decides what the generated types are called.</param>
/// <param name="typeNames">Keeps the generated names from colliding. Share this with an endpoint generator writing to the same package.</param>
public class DtoGenerator(INamingStrategy naming, TypeNameRegistry? typeNames = null)
{
    /// <summary>
    /// Generates a type for each of the <paramref name="schemas"/> that needs one.
    /// </summary>
    public IEnumerable<IJvmType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
    {
        var names = typeNames ?? new TypeNameRegistry();

        // Create all builders first, so that types from the document claim their names before the types
        // generated for inline schemas, which get a number appended if their name is already taken
        var builders = schemas.Select(x => DtoBuilder.For(x.Key, x.Value, naming, names))
                              .OfType<DtoBuilder>()
                              .ToList();

        foreach (var builder in builders)
        {
            foreach (var type in builder.BuildTypes())
                yield return type;
        }
    }
}
