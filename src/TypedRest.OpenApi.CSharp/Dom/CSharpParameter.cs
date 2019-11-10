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
        {
            switch (Value)
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
