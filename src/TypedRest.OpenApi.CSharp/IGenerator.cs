using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface IGenerator
    {
        INamingConvention Naming { get; }

        bool GenerateInterfaces { get; }

        (CSharpProperty property, IEnumerable<CSharpType> types) GenerateEndpoint(string key, IEndpoint endpoint);
    }
}
