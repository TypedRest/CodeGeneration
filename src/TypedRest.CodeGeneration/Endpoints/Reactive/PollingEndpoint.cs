using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Endpoints.Reactive;

/// <summary>
/// Endpoint for a resource that can be polled for state changes.
/// </summary>
public class PollingEndpoint : ElementEndpoint
{
    public override string Kind => "polling";
}
