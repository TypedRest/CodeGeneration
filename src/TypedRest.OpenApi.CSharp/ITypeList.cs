using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface ITypeList : IEnumerable<CSharpType>
    {
        [NotNull]
        CSharpIdentifier ImplementationFor(IEndpoint endpoint);

        [NotNull]
        CSharpIdentifier InterfaceFor(IEndpoint endpoint);

        [NotNull]
        CSharpIdentifier For(OpenApiSchema schema);
    }
}
