using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface ITypeList : IEnumerable<CSharpType>
    {
        CSharpIdentifier ImplementationFor(IEndpoint endpoint);

        CSharpIdentifier InterfaceFor(IEndpoint endpoint);

        CSharpIdentifier DtoFor(OpenApiSchema schema);
    }
}
