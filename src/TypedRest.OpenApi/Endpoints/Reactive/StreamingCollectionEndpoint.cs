using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a collection of entities observable as an append-only stream using long-polling.
    /// </summary>
    public class StreamingCollectionEndpoint : CollectionEndpoint
    {
        public override string Kind => "streaming-collection";
    }
}
