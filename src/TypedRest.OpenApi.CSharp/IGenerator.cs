using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface IGenerator
    {
        INamingStrategy Naming { get; }

        bool GenerateInterfaces { get; }

        (CSharpProperty property, IEnumerable<ICSharpType> types) GetEndpoints(string key, IEndpoint endpoint);
    }
}
