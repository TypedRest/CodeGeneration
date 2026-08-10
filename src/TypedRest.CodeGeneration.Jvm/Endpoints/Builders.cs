using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Endpoints.Raw;
using TypedRest.CodeGeneration.Endpoints.Reactive;
using TypedRest.CodeGeneration.Endpoints.Rpc;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm.Endpoints;

/// <summary>
/// Builds code for the entry endpoint.
/// </summary>
public class EntryBuilder : BuilderBase<EntryEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(EntryEndpoint endpoint, IEndpointGenerator generator)
        => Packages.EntryEndpoint;

    /// <inheritdoc/>
    protected override JvmConstructor BuildConstructor(EntryEndpoint endpoint, List<JvmExpression> extraArguments, IEndpointGenerator generator)
        => new();
}

/// <summary>
/// Builds code for endpoints with no more specific kind.
/// </summary>
public class DefaultBuilder : BuilderBase<Endpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(Endpoint endpoint, IEndpointGenerator generator)
        => Packages.AbstractEndpoint;

    /// <inheritdoc/>
    /// <remarks>
    /// <c>AbstractEndpoint</c> is abstract and there is no <c>EndpointImpl</c> to instantiate instead, so a plain
    /// endpoint needs a class of its own even when it has no children to hold.
    /// </remarks>
    protected override bool RequiresGeneratedClass => true;

    /// <inheritdoc/>
    /// <remarks>
    /// <c>AbstractEndpoint</c> has no secondary constructor taking the relative URI as a <c>String</c>, unlike
    /// every <c>Impl</c> class.
    /// </remarks>
    protected override bool RequiresUriObject => true;
}

/// <summary>
/// Builds code for <see cref="ElementEndpoint"/>s.
/// </summary>
public class ElementBuilder : BuilderBase<ElementEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(ElementEndpoint endpoint, IEndpointGenerator generator)
        => ElementEndpointType(endpoint.Schema, generator);

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(ElementEndpoint endpoint, IEndpointGenerator generator)
        => [new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema))];

    internal static JvmIdentifier ElementEndpointType(OpenApiSchema? schema, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Generic, "ElementEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(schema));
}

/// <summary>
/// Builds code for <see cref="IndexerEndpoint"/>s.
/// </summary>
public class IndexerBuilder : BuilderBase<IndexerEndpoint>
{
    /// <inheritdoc/>
    protected override (JvmIdentifier baseType, IEnumerable<IJvmType> types, IEnumerable<JvmExpression> extraArguments) GetBase(string key, IndexerEndpoint endpoint, IEndpointGenerator generator)
    {
        var (child, types) = generator.Generate(
            EndpointTree.ElementKey(key),
            endpoint.Element ?? throw new InvalidOperationException($"Missing element for {endpoint}."));

        return (
            Packages.Implementation(Packages.Generic, "IndexerEndpoint").WithTypeArguments(child.Type),
            types,
            [ElementFactory(child.Type)]);
    }

    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(IndexerEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Generic, "IndexerEndpoint");

    /// <summary>
    /// Builds the factory the indexer and collection endpoints use to create an endpoint per element.
    /// </summary>
    internal static JvmExpression ElementFactory(JvmIdentifier elementType)
        => new JvmElementFactory(
            new JvmNew(elementType)
            {
                Arguments =
                {
                    new JvmName(JvmElementFactory.ReferrerParameter),
                    new JvmName(JvmElementFactory.RelativeUriParameter)
                }
            });
}

/// <summary>
/// Common base class for builders for <see cref="CollectionEndpoint"/> and derived types.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
public abstract class CollectionBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
    where TEndpoint : CollectionEndpoint
{
    /// <inheritdoc/>
    protected override (JvmIdentifier baseType, IEnumerable<IJvmType> types, IEnumerable<JvmExpression> extraArguments) GetBase(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        var entity = generator.Naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."));
        var entityClass = new JvmClassLiteral(entity);

        if (endpoint.Element == null)
            return (CollectionType(entity), [], [entityClass]);

        endpoint.Element.Schema ??= endpoint.Schema;

        // TElementEndpoint is constrained to ElementEndpoint<TEntity>, so the two schemas have to agree
        var elementEntity = generator.Naming.TypeFor(endpoint.Element.Schema);
        if (elementEntity.ToString() != entity.ToString())
        {
            generator.Log.Report(Messages.ElementSchemaMismatch(key, entity.ToString(), elementEntity.ToString()));
            endpoint.Element.Schema = endpoint.Schema;
        }

        var (child, types) = generator.Generate(EndpointTree.ElementKey(key), endpoint.Element);
        var elementType = child.Type;

        // A plain element endpoint needs no class of its own; the specialized collection endpoint creates it
        if (IsPlainElementEndpoint(elementType, entity))
            return (CollectionType(entity), types, [entityClass]);

        return (
            GenericCollectionType(entity, elementType),
            types,
            [entityClass, IndexerBuilder.ElementFactory(elementType)]);
    }

    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(TEndpoint endpoint, IEndpointGenerator generator)
        => CollectionType(generator.Naming.TypeFor(endpoint.Schema));

    /// <summary>
    /// The name of the TypedRest type creating its element endpoints itself, e.g. <c>CollectionEndpoint</c>.
    /// </summary>
    protected abstract string TypeName { get; }

    /// <summary>
    /// The name of the TypedRest type taking a factory for element endpoints, e.g. <c>GenericCollectionEndpoint</c>.
    /// </summary>
    protected abstract string GenericTypeName { get; }

    /// <summary>
    /// The package the two types live in.
    /// </summary>
    protected virtual JvmPackage Package => Packages.Generic;

    private JvmIdentifier CollectionType(JvmIdentifier entity)
        => Packages.Implementation(Package, TypeName).WithTypeArguments(entity);

    private JvmIdentifier GenericCollectionType(JvmIdentifier entity, JvmIdentifier elementType)
        => Packages.Implementation(Package, GenericTypeName).WithTypeArguments(entity, elementType);

    private bool IsPlainElementEndpoint(JvmIdentifier elementType, JvmIdentifier entity)
        => elementType.Name == "ElementEndpointImpl"
        && Equals(elementType.Package, Packages.Generic)
        && elementType.TypeArguments.Count == 1
        && elementType.TypeArguments[0].ToString() == entity.ToString();
}

/// <summary>
/// Builds code for <see cref="CollectionEndpoint"/>s.
/// </summary>
public class CollectionBuilder : CollectionBuilderBase<CollectionEndpoint>
{
    /// <inheritdoc/>
    protected override string TypeName => "CollectionEndpoint";

    /// <inheritdoc/>
    protected override string GenericTypeName => "GenericCollectionEndpoint";
}

/// <summary>
/// Builds code for <see cref="StreamingCollectionEndpoint"/>s.
/// </summary>
public class StreamingCollectionBuilder : CollectionBuilderBase<StreamingCollectionEndpoint>
{
    /// <inheritdoc/>
    protected override string TypeName => "StreamingCollectionEndpoint";

    /// <inheritdoc/>
    protected override string GenericTypeName => "GenericStreamingCollectionEndpoint";

    /// <inheritdoc/>
    protected override JvmPackage Package => Packages.Reactive;
}

/// <summary>
/// Builds code for <see cref="ActionEndpoint"/>s.
/// </summary>
public class ActionBuilder : BuilderBase<ActionEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(ActionEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Rpc, "ActionEndpoint");
}

/// <summary>
/// Builds code for <see cref="ProducerEndpoint"/>s.
/// </summary>
public class ProducerBuilder : BuilderBase<ProducerEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(ProducerEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Rpc, "ProducerEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(endpoint.Schema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(ProducerEndpoint endpoint, IEndpointGenerator generator)
        => [new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema))];
}

/// <summary>
/// Builds code for <see cref="ConsumerEndpoint"/>s.
/// </summary>
public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(ConsumerEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Rpc, "ConsumerEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(endpoint.Schema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(ConsumerEndpoint endpoint, IEndpointGenerator generator)
        => [new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema))];
}

/// <summary>
/// Builds code for <see cref="FunctionEndpoint"/>s.
/// </summary>
public class FunctionBuilder : BuilderBase<FunctionEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(FunctionEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Rpc, "FunctionEndpoint")
                   .WithTypeArguments(
                        generator.Naming.TypeFor(endpoint.RequestSchema),
                        generator.Naming.TypeFor(endpoint.ResponseSchema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(FunctionEndpoint endpoint, IEndpointGenerator generator)
        =>
        [
            new JvmClassLiteral(generator.Naming.TypeFor(endpoint.RequestSchema)),
            new JvmClassLiteral(generator.Naming.TypeFor(endpoint.ResponseSchema))
        ];
}

/// <summary>
/// Builds code for <see cref="BlobEndpoint"/>s.
/// </summary>
public class BlobBuilder : BuilderBase<BlobEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(BlobEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Raw, "BlobEndpoint");
}

/// <summary>
/// Builds code for <see cref="UploadEndpoint"/>s.
/// </summary>
public class UploadBuilder : BuilderBase<UploadEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(UploadEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Raw, "UploadEndpoint");

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(UploadEndpoint endpoint, IEndpointGenerator generator)
        => endpoint.FormField is {Length: > 0} field ? [new JvmLiteral(field)] : [];
}

/// <summary>
/// Builds code for <see cref="PollingEndpoint"/>s.
/// </summary>
public class PollingBuilder : BuilderBase<PollingEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(PollingEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Reactive, "PollingEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(endpoint.Schema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(PollingEndpoint endpoint, IEndpointGenerator generator)
        => [new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema))];
}

/// <summary>
/// Builds code for <see cref="StreamingEndpoint"/>s.
/// </summary>
public class StreamingBuilder : BuilderBase<StreamingEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(StreamingEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Reactive, "StreamingEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(endpoint.Schema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(StreamingEndpoint endpoint, IEndpointGenerator generator)
    {
        yield return new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema));

        // The separator defaults to "\n", so it is only worth passing when the document asks for another one
        if (endpoint.Separator is {Length: > 0} separator && separator != "\n")
            yield return new JvmLiteral(separator);
    }
}

/// <summary>
/// Builds code for <see cref="SseStreamingEndpoint"/>s.
/// </summary>
public class SseStreamingBuilder : BuilderBase<SseStreamingEndpoint>
{
    /// <inheritdoc/>
    protected override JvmIdentifier GetBaseType(SseStreamingEndpoint endpoint, IEndpointGenerator generator)
        => Packages.Implementation(Packages.Reactive, "SseStreamingEndpoint")
                   .WithTypeArguments(generator.Naming.TypeFor(endpoint.Schema));

    /// <inheritdoc/>
    protected override IEnumerable<JvmExpression> ExtraArguments(SseStreamingEndpoint endpoint, IEndpointGenerator generator)
    {
        yield return new JvmClassLiteral(generator.Naming.TypeFor(endpoint.Schema));

        if (endpoint.EventType is {Length: > 0} eventType)
            yield return new JvmLiteral(eventType);
    }
}
