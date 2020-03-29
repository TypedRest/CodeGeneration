using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="CollectionEndpoint"/>s.
    /// </summary>
    public class CollectionBuilder : CollectionBuilderBase<CollectionEndpoint>
    {
        protected override string TypeNamespace => Namespace.Name;

        protected override string TypeName => "CollectionEndpoint";
    }
}
