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

        [CanBeNull]
        public string Description { get; set; }

        public CSharpProperty([NotNull] CSharpIdentifier type, [NotNull] string name)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [CanBeNull]
        public CSharpClassConstruction GetterExpression { get; set; }

        public bool HasSetter { get; set; }

        [NotNull, ItemNotNull]
        public IEnumerable<string> GetNamespaces()
        {
            foreach (string ns in Type.GetNamespaces())
                yield return ns;

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
                ? propertyDeclaration.WithAccessorList(AccessorList(List(GetAccessors())))
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

        [NotNull, ItemNotNull]
        private IEnumerable<AccessorDeclarationSyntax> GetAccessors()
        {
            AccessorDeclarationSyntax Declaration(SyntaxKind kind) => AccessorDeclaration(kind).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            yield return Declaration(SyntaxKind.GetAccessorDeclaration);
            if (HasSetter) yield return Declaration(SyntaxKind.SetAccessorDeclaration);
        }
    }
}
