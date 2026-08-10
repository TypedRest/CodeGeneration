using TypedRest.CodeGeneration.Endpoints.Raw;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Raw;

/// <summary>
/// Builds TypeScript code for <see cref="BlobEndpoint"/>s.
/// </summary>
public class BlobBuilder : BuilderBase<BlobEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(BlobEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Raw, "BlobEndpoint");
}

/// <summary>
/// Builds TypeScript code for <see cref="UploadEndpoint"/>s.
/// </summary>
public class UploadBuilder : BuilderBase<UploadEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, UploadEndpoint endpoint, IEndpointGenerator generator)
        => (GetBaseType(endpoint, generator),
            [],
            endpoint.FormField == null ? [] : [new TsLiteral(endpoint.FormField)]);

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(UploadEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Raw, "UploadEndpoint");
}
