using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpProperty
    {
        [NotNull]
        public CSharpIdentifier Type { get; }

        [NotNull]
        public string Name { get; }

        public CSharpProperty([NotNull] CSharpIdentifier type, [NotNull] string name)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [CanBeNull]
        public CSharpClassConstruction GetterExpression { get; set; }

        private static readonly AccessorListSyntax _autoGetterAndSetter = AccessorList(
            List(new[]{
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))}));

        [NotNull, ItemNotNull]
        public IEnumerable<string> GetNamespaces()
        {
            if (!string.IsNullOrEmpty(Type.Namespace))
                yield return Type.Namespace;

            if (GetterExpression != null)
            {
                foreach (string ns in GetterExpression.GetNamespaces())
                    yield return ns;
            }
        }

        [NotNull]
        public PropertyDeclarationSyntax ToSyntax()
        {
            var propertyDeclaration = PropertyDeclaration(GetTypeIdentifier(), Identifier(Name))
               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)));

            return (GetterExpression == null)
                ? propertyDeclaration.WithAccessorList(_autoGetterAndSetter)
                : propertyDeclaration.WithExpressionBody(ArrowExpressionClause(GetterExpression.ToNewSyntax()))
                                     .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        [NotNull]
        private TypeSyntax GetTypeIdentifier()
        {
            switch (Type.Name)
            {
                case "string":
                    return PredefinedType(Token(SyntaxKind.StringKeyword));
                default:
                    return IdentifierName(Type.Name);
            }
        }
    }
}
