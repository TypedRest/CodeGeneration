using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpParameter
    {
        public CSharpIdentifier Type { get; }

        public string Name { get; }

        public object? Value { get; set; }

        public bool ThisReference { get; set; }

        public CSharpParameter(CSharpIdentifier type, string name)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ParameterSyntax ToParameterSyntax()
            => Parameter(Identifier(Name)).WithType(Type.ToSyntax());

        public ArgumentSyntax ToArgumentSyntax()
        {
            if (ThisReference)
                return Argument(ThisExpression());

            var identifierName = IdentifierName(Name);
            var literal = GetLiteralExpression();
            return literal == null
                ? Argument(identifierName)
                : Argument(literal).WithNameColon(NameColon(identifierName));
        }

        private LiteralExpressionSyntax? GetLiteralExpression()
            => Value switch
            {
                bool value => LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                int value => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)),
                long value => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)),
                float value => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)),
                double value => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)),
                string value => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value)),
                _ => null
            };
    }
}
