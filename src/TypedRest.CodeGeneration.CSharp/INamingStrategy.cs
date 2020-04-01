using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp
{
    public interface INamingStrategy
    {
        string Property(string key);

        CSharpIdentifier EndpointType(string key, IEndpoint endpoint);

        CSharpIdentifier DtoType(string key);

        CSharpIdentifier TypeFor(OpenApiSchema schema);
    }
}
