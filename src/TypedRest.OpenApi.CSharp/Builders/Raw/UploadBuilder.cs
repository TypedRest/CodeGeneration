using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Raw;

namespace TypedRest.OpenApi.CSharp.Builders.Raw
{
    /// <summary>
    /// Builds C# code snippets for <see cref="UploadEndpoint"/>s.
    /// </summary>
    public class UploadBuilder : BuilderBase<UploadEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(UploadEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "UploadEndpoint");

        protected override IEnumerable<CSharpParameter> GetParameters(UploadEndpoint endpoint)
        {
            foreach (var parameter in base.GetParameters(endpoint))
                yield return parameter;

            if (!string.IsNullOrEmpty(endpoint.FormField))
                yield return new CSharpParameter(CSharpIdentifier.String, "formField", endpoint.FormField);
        }
    }
}
