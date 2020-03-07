using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpDto : ICSharpType
    {
        private readonly CompilationUnitSyntax _syntax;

        public CSharpDto(CSharpIdentifier identifier, OpenApiSchema schema)
        {
            Identifier = identifier;

            string sourceCode = new CSharpGenerator(schema.ToNJsonSchema())
            {
                Settings =
                {
                    Namespace = identifier.Namespace,
                    SchemaType = SchemaType.OpenApi3
                }
            }.GenerateFile(identifier.Name);
            _syntax = ParseCompilationUnit(sourceCode);
        }

        public CSharpIdentifier Identifier { get; }

        public CompilationUnitSyntax ToSyntax()
            => _syntax;
    }
}
