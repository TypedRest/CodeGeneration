using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClassConstruction
    {
        public CSharpIdentifier Type { get; }

        public CSharpClassConstruction(CSharpIdentifier type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public List<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();

        public IEnumerable<string> GetNamespaces()
        {
            foreach (string ns in Type.GetNamespaces())
                yield return ns;

            foreach (string ns in Parameters.SelectMany(x => x.Type.GetNamespaces()))
                yield return ns;
        }

        public ObjectCreationExpressionSyntax ToNewSyntax()
            => ObjectCreationExpression(Type.ToSyntax())
               .WithArgumentList(GetArgumentList());

        public ConstructorDeclarationSyntax ToConstructorSyntax(string typeName)
            => ConstructorDeclaration(Identifier(typeName))
              .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
              .WithParameterList(GetParameterList())
              .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, GetArgumentList()))
              .WithBody(Block());

        private ArgumentListSyntax GetArgumentList()
            => ArgumentList(SeparatedList(
                Parameters.Select(x => x.ToArgumentSyntax())));

        private ParameterListSyntax GetParameterList()
            => ParameterList(SeparatedList(
                Parameters.Where(x => x.Value == null).Select(x => x.ToParameterSyntax())));
    }
}
