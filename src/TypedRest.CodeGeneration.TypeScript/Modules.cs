using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// The modules of the TypedRest runtime library that generated code imports from.
/// </summary>
/// <param name="package">The name of the npm package providing TypedRest.</param>
/// <remarks>
/// TypedRest for TypeScript has no <c>exports</c> map, so subpath imports resolve by directory. This mirrors the
/// namespace layout of TypedRest for .NET and keeps the imported names short.
/// </remarks>
public class Modules(string package = Modules.DefaultPackage)
{
    /// <summary>
    /// The name of the npm package providing TypedRest.
    /// </summary>
    public const string DefaultPackage = "typedrest";

    /// <summary>
    /// The default modules, importing from the <c>typedrest</c> package.
    /// </summary>
    public static Modules Default { get; } = new();

    /// <summary>
    /// The name of the npm package providing TypedRest.
    /// </summary>
    public string Package { get; } = package;

    /// <summary>
    /// Holds <c>Endpoint</c> and <c>EntryEndpoint</c>.
    /// </summary>
    public TsModule Endpoints { get; } = TsModule.External($"{package}/endpoints");

    /// <summary>
    /// Holds <c>ElementEndpoint</c>, <c>CollectionEndpoint</c>, <c>GenericCollectionEndpoint</c> and <c>IndexerEndpoint</c>.
    /// </summary>
    public TsModule Generic { get; } = TsModule.External($"{package}/endpoints/generic");

    /// <summary>
    /// Holds <c>ActionEndpoint</c>, <c>ProducerEndpoint</c>, <c>ConsumerEndpoint</c> and <c>FunctionEndpoint</c>.
    /// </summary>
    public TsModule Rpc { get; } = TsModule.External($"{package}/endpoints/rpc");

    /// <summary>
    /// Holds <c>BlobEndpoint</c> and <c>UploadEndpoint</c>.
    /// </summary>
    public TsModule Raw { get; } = TsModule.External($"{package}/endpoints/raw");
}
