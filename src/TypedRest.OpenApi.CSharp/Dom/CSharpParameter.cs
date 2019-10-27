using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpParameter
    {
        [NotNull]
        public CSharpIdentifier Type { get; }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public object LiteralValue { get; }

        public CSharpParameter([NotNull] CSharpIdentifier type, [NotNull] string name, [CanBeNull] object literalValue = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            LiteralValue = literalValue;
        }

        public ParameterSyntax ToParameterSyntax()
            => Parameter(Identifier(Name)).WithType(Type.ToSyntax());

        public ArgumentSyntax ToArgumentSyntax()
        {
            var identifierName = IdentifierName(Name);
            var expression = GetLiteralExpression();
            return expression == null
                ? Argument(identifierName)
                : Argument(expression).WithNameColon(NameColon(identifierName));
        }

        private LiteralExpressionSyntax GetLiteralExpression()
        {
            switch (LiteralValue)
            {
                case string value:
                    return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value));

                case int value:
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value));

                default:
                    return null;
            }
        }
    }
}
