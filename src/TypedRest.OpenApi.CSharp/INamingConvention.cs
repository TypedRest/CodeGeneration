using JetBrains.Annotations;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface INamingConvention
    {
        [NotNull]
        string Property([NotNull] string key);

        [NotNull]
        CSharpIdentifier EndpointType([NotNull] string key, [NotNull] IEndpoint endpoint);
    }
}
