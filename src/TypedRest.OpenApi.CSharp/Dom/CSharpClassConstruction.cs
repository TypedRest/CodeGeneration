using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClassConstruction
    {
        [NotNull]
        public CSharpIdentifier Type { get; }

        public CSharpClassConstruction([NotNull] CSharpIdentifier type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        [NotNull, ItemNotNull]
        public List<CSharpParameter> Parameters { get; } = new List<CSharpParameter>();

        [NotNull, ItemNotNull]
        public IEnumerable<string> GetNamespaces()
        {
            foreach (string ns in Type.GetNamespaces())
                yield return ns;

            foreach (string ns in Parameters.SelectMany(x => x.Type.GetNamespaces()))
                yield return ns;
        }

        [NotNull]
        public ObjectCreationExpressionSyntax ToNewSyntax()
            => ObjectCreationExpression(IdentifierName(Type.Name))
               .WithArgumentList(GetArgumentList());

        [NotNull] public ConstructorDeclarationSyntax ToConstructorSyntax(string typeName)
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
                Parameters.Where(x => x.LiteralValue == null).Select(x => x.ToParameterSyntax())));
    }
}
