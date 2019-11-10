using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public abstract class CSharpType
    {
        public abstract CSharpIdentifier Identifier { get; }

        public List<CSharpIdentifier> Interfaces { get; } = new List<CSharpIdentifier>();

        public string? Description { get; set; }

        public List<CSharpProperty> Properties { get; } = new List<CSharpProperty>();

        public CompilationUnitSyntax ToSyntax()
            => CompilationUnit()
              .WithUsings(List(GetNamespaces().Select(x => UsingDirective(IdentifierName(x)))))
              .AddMembers(NamespaceDeclaration(IdentifierName(Identifier.Namespace)).AddMembers(GetTypeDeclaration()))
              .NormalizeWhitespace();

        protected virtual ISet<string> GetNamespaces()
        {
            var namespaces = new SortedSet<string>();

            foreach (string ns in Interfaces.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            foreach (string ns in Properties.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            if (!string.IsNullOrEmpty(Identifier.Namespace))
                namespaces.Remove(Identifier.Namespace);

            return namespaces;
        }

        protected abstract MemberDeclarationSyntax GetTypeDeclaration();
    }
}
