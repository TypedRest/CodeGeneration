using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public static class SyntaxExtensions
    {
        public static TSyntax WithDocumentation<TSyntax>([NotNull] this TSyntax node, [CanBeNull] string summary)
            where TSyntax : SyntaxNode
        {
            if (string.IsNullOrEmpty(summary)) return node;

            return node.WithLeadingTrivia(TriviaList(Trivia(DocumentationCommentTrivia(
                SyntaxKind.MultiLineDocumentationCommentTrivia,
                List(new XmlNodeSyntax[]
                {
                    XmlText().WithTextTokens(TokenList(XmlTextLiteral(TriviaList(DocumentationCommentExterior("///")), " ", " ", TriviaList()))),
                    XmlSummaryElement(XmlText().WithTextTokens(TokenList(XmlTextNewLine("\n"), XmlTextLiteral(" " + summary), XmlTextNewLine("\n"), XmlTextLiteral(" ")))),
                    XmlText().WithTextTokens(TokenList(XmlTextNewLine("\n", continueXmlDocumentationComment: false)))
                })))));
        }
    }
}
