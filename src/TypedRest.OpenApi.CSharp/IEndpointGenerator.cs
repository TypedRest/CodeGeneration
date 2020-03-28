using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface IEndpointGenerator
    {
        INamingStrategy Naming { get; }

        bool WithInterfaces { get; }

        (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint);
    }
}
