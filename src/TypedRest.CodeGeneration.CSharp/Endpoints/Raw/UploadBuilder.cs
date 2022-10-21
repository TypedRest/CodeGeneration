using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Raw;

/// <summary>
/// Builds C# code snippets for <see cref="UploadEndpoint"/>s.
/// </summary>
public class UploadBuilder : BuilderBase<UploadEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(UploadEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "UploadEndpoint");

    protected override IEnumerable<CSharpParameter> GetParameters(UploadEndpoint endpoint)
    {
        foreach (var parameter in base.GetParameters(endpoint))
            yield return parameter;

        if (!string.IsNullOrEmpty(endpoint.FormField))
            yield return new CSharpParameter(CSharpIdentifier.String, "formField") {Value = endpoint.FormField};
    }
}
