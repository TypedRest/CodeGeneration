using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Builds TypeScript code for <see cref="EntryEndpoint"/>s.
/// </summary>
public class EntryBuilder : BuilderBase<EntryEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(EntryEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Endpoints, "EntryEndpoint");

    /// <inheritdoc/>
    /// <remarks>
    /// The inherited constructor already takes the base URI plus optional serializer, error handler, link
    /// extractor and HTTP client, so there is nothing worth generating over it. This also makes the C#
    /// generator's <c>GenerateEntryConstructor</c> option unnecessary here.
    /// </remarks>
    protected override TsConstructor? BuildConstructor(EntryEndpoint endpoint, List<TsExpression> extraArguments, IEndpointGenerator generator)
        => null;
}
