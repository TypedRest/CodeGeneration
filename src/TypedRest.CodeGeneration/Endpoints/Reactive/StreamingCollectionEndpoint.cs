using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a collection of entities observable as an append-only stream using long-polling.
    /// </summary>
    public class StreamingCollectionEndpoint : CollectionEndpoint
    {
        public override string Kind => "streaming-collection";
    }
}
