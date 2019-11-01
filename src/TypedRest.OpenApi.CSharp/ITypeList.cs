using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public interface ITypeList : IEnumerable<CSharpType>
    {
        CSharpIdentifier this[[NotNull] IEndpoint endpoint] { get; }

        CSharpIdentifier this[[NotNull] OpenApiSchema schema] { get; }
    }
}
