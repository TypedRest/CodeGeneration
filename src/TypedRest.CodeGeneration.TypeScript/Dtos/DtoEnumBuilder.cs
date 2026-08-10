using System.Globalization;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Dtos;

/// <summary>
/// Builds a TypeScript literal union type alias for a schema with an <c>enum</c>.
/// </summary>
/// <inheritdoc cref="DtoBuilder"/>
/// <remarks>
/// A TypeScript <c>enum</c> would be the closer analogue of the C# output, but a string <c>enum</c> refuses
/// assignment of a bare string literal - which is exactly what <c>JSON.parse()</c> produces. Unions accept the
/// literals directly and are fully erased at runtime.
/// </remarks>
public class DtoEnumBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
    : DtoBuilder(key, schema, naming, typeNames)
{
    /// <inheritdoc/>
    protected override ITsType BuildTypeInner()
    {
        var literals = new List<string>();

        foreach (var value in Schema.Enum)
        {
            string? literal = value switch
            {
                OpenApiString {Value: not null} str => Ts.Quote(str.Value),
                OpenApiInteger num => num.Value.ToString(CultureInfo.InvariantCulture),
                OpenApiLong num => num.Value.ToString(CultureInfo.InvariantCulture),
                _ => null
            };

            // Unlike C# enum members, duplicate literals are not an error - they are just redundant
            if (literal != null && !literals.Contains(literal)) literals.Add(literal);
        }

        return new TsTypeAlias(Identifier, literals.Count == 0 ? "never" : string.Join(" | ", literals));
    }
}
