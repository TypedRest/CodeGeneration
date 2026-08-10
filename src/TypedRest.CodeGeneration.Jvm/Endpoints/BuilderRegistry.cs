using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.Jvm.Endpoints;

/// <summary>
/// A list of all known <see cref="IBuilder"/>s.
/// </summary>
public class BuilderRegistry : BuilderRegistry<IBuilder>
{
    /// <summary>
    /// Builder registry with the built-in default <see cref="IBuilder"/>s.
    /// </summary>
    public static BuilderRegistry Default
        => new BuilderRegistry()
          .Add(new DefaultBuilder())
          .Add(new ElementBuilder())
          .Add(new IndexerBuilder())
          .Add(new CollectionBuilder())
          .Add(new ActionBuilder())
          .Add(new ProducerBuilder())
          .Add(new ConsumerBuilder())
          .Add(new FunctionBuilder())
          .Add(new UploadBuilder())
          .Add(new BlobBuilder())
          .Add(new PollingBuilder())
          .Add(new StreamingBuilder())
          .Add(new SseStreamingBuilder())
          .Add(new StreamingCollectionBuilder());

    /// <summary>
    /// Creates a registry holding only the <see cref="EntryBuilder"/>.
    /// </summary>
    public BuilderRegistry()
    {
        // Must always be registered
        Add(new EntryBuilder());
    }

    /// <summary>
    /// Adds <paramref name="builder"/> to the list of known builders.
    /// </summary>
    public BuilderRegistry Add<TEndpoint>(IBuilder<TEndpoint> builder)
        where TEndpoint : IEndpoint, new()
    {
        Register<TEndpoint>(builder);
        return this;
    }
}
