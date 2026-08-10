using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Generic;

/// <summary>
/// Builds TypeScript code for <see cref="ElementEndpoint"/>s.
/// </summary>
public class ElementBuilder : BuilderBase<ElementEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(ElementEndpoint endpoint, IEndpointGenerator generator)
        => ElementEndpointType(endpoint.Schema, generator);

    internal static TsIdentifier ElementEndpointType(OpenApiSchema? schema, IEndpointGenerator generator)
        => new(generator.Modules.Generic, "ElementEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(schema)}
        };
}
