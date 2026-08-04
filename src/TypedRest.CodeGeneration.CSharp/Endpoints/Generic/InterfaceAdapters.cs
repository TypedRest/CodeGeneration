using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Generic;

/// <summary>
/// Builds explicit interface implementations that adapt members inherited from a concrete endpoint base class to
/// the element endpoint interfaces used by generated endpoint interfaces.
/// </summary>
/// <remarks>
/// <c>IIndexerEndpoint{TElementEndpoint}</c> and friends are covariant, but C# still requires an exact signature
/// match to implement them. This mirrors what TypedRest's own <c>CollectionEndpoint{TEntity}</c> does by hand.
/// </remarks>
internal static class InterfaceAdapters
{
    /// <summary>
    /// <c>TElement IIndexerEndpoint&lt;TElement&gt;.this[string id] =&gt; this[id];</c>
    /// </summary>
    public static CSharpIndexer IndexerById(CSharpIdentifier elementInterface)
        => new(elementInterface, new CSharpParameter(CSharpIdentifier.String, "id"))
        {
            ExplicitInterface = new CSharpIdentifier(Namespace.Name, "IIndexerEndpoint") {TypeArguments = {elementInterface}},
            GetterExpression = "this[id]"
        };

    /// <summary>
    /// <c>TElement ICollectionEndpoint&lt;TEntity, TElement&gt;.this[TEntity entity] =&gt; this[entity];</c>
    /// </summary>
    public static CSharpIndexer IndexerByEntity(CSharpIdentifier entity, CSharpIdentifier elementInterface)
        => new(elementInterface, new CSharpParameter(entity, "entity"))
        {
            ExplicitInterface = CollectionInterface(entity, elementInterface),
            GetterExpression = "this[entity]"
        };

    /// <summary>
    /// <c>ITask&lt;TElement?&gt; ICollectionEndpoint&lt;TEntity, TElement&gt;.CreateAsync(TEntity entity, CancellationToken cancellationToken) =&gt; CreateAsync(entity, cancellationToken);</c>
    /// </summary>
    public static CSharpMethod CreateAsync(CSharpIdentifier entity, CSharpIdentifier elementInterface)
        => new(new CSharpIdentifier("MorseCode.ITask", "ITask") {TypeArguments = {elementInterface.ToNullable()}}, "CreateAsync")
        {
            ExplicitInterface = CollectionInterface(entity, elementInterface),
            Parameters =
            {
                new CSharpParameter(entity, "entity"),
                new CSharpParameter(new CSharpIdentifier("System.Threading", "CancellationToken"), "cancellationToken")
            },
            BodyExpression = "CreateAsync(entity, cancellationToken)"
        };

    private static CSharpIdentifier CollectionInterface(CSharpIdentifier entity, CSharpIdentifier elementInterface)
        => new(Namespace.Name, "ICollectionEndpoint") {TypeArguments = {entity, elementInterface}};
}
