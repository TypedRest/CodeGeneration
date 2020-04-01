using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Builders.Generic
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
