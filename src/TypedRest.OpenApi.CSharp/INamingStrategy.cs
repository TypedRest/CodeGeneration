using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface INamingStrategy
    {
        string Property(string key);

        CSharpIdentifier EndpointType(string key, IEndpoint endpoint);

        CSharpIdentifier DtoType(string key);

        CSharpIdentifier TypeFor(OpenApiSchema schema);
    }
}
