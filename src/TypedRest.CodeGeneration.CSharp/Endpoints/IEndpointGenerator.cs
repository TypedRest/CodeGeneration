using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints
{
    public interface IEndpointGenerator
    {
        INamingStrategy Naming { get; }

        bool WithInterfaces { get; }

        (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint);
    }
}
