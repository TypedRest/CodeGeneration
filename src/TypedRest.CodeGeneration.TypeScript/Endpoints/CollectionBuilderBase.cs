using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="CollectionEndpoint"/> and derived types.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
public abstract class CollectionBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
    where TEndpoint : CollectionEndpoint
{
    /// <inheritdoc/>
    /// <remarks>
    /// TypeScript splits the two arities of the C# <c>CollectionEndpoint</c> into two classes:
    /// <c>CollectionEndpoint&lt;TEntity&gt;</c>, which creates plain element endpoints itself, and
    /// <c>GenericCollectionEndpoint&lt;TEntity, TElementEndpoint&gt;</c>, which takes the element endpoint class
    /// as a constructor argument.
    /// </remarks>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        var entity = generator.Naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."));

        if (endpoint.Element == null)
            return (CollectionType(generator, entity), [], []);

        endpoint.Element.Schema ??= endpoint.Schema;

        // TElementEndpoint is constrained to ElementEndpoint<TEntity>, so the two schemas have to agree
        var elementEntity = generator.Naming.TypeFor(endpoint.Element.Schema);
        if (elementEntity.ToTypeExpression() != entity.ToTypeExpression())
        {
            generator.Log.Report(Messages.ElementSchemaMismatch(key, entity.ToTypeExpression(), elementEntity.ToTypeExpression()));
            endpoint.Element.Schema = endpoint.Schema;
        }

        var (getter, types) = generator.Generate(EndpointTree.ElementKey(key), endpoint.Element);
        var elementType = getter.Type;

        // A plain element endpoint needs no class of its own; the specialized CollectionEndpoint<TEntity> creates it
        if (IsPlainElementEndpoint(elementType, entity, generator))
            return (CollectionType(generator, entity), types, []);

        return (
            new TsIdentifier(generator.Modules.Generic, GenericTypeName) {TypeArguments = {entity, elementType}},
            types,
            [new TsTypeRef(elementType)]);
    }

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(TEndpoint endpoint, IEndpointGenerator generator)
        => CollectionType(generator, generator.Naming.TypeFor(endpoint.Schema));

    /// <summary>
    /// The name of the TypedRest type creating its element endpoints itself, e.g. <c>CollectionEndpoint</c>.
    /// </summary>
    protected abstract string TypeName { get; }

    /// <summary>
    /// The name of the TypedRest type taking the element endpoint class as a constructor argument,
    /// e.g. <c>GenericCollectionEndpoint</c>.
    /// </summary>
    protected abstract string GenericTypeName { get; }

    private TsIdentifier CollectionType(IEndpointGenerator generator, TsIdentifier entity)
        => new(generator.Modules.Generic, TypeName) {TypeArguments = {entity}};

    private static bool IsPlainElementEndpoint(TsIdentifier elementType, TsIdentifier entity, IEndpointGenerator generator)
        => elementType.Name == "ElementEndpoint"
        && Equals(elementType.Module, generator.Modules.Generic)
        && elementType.TypeArguments.Count == 1
        && elementType.TypeArguments[0].ToTypeExpression() == entity.ToTypeExpression();
}
