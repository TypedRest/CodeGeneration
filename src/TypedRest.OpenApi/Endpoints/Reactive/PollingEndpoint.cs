using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a resource that can be polled for state changes.
    /// </summary>
    public class PollingEndpoint : ElementEndpoint
    {
        public override string Type => "polling";
    }
}
