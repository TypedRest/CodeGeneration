using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public interface ICSharpType
    {
        CSharpIdentifier Identifier { get; }

        CompilationUnitSyntax ToSyntax();
    }
}
