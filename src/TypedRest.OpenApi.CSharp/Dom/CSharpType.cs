using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public abstract class CSharpType
    {
        [NotNull]
        public abstract CSharpIdentifier Identifier { get; }

        [NotNull, ItemNotNull]
        public List<CSharpIdentifier> Interfaces { get; } = new List<CSharpIdentifier>();

        [CanBeNull]
        public string Description { get; set; }

        [NotNull, ItemNotNull]
        public List<CSharpProperty> Properties { get; } = new List<CSharpProperty>();

        [NotNull]
        public CompilationUnitSyntax ToSyntax()
            => CompilationUnit()
              .WithUsings(List(GetNamespaces().Select(x => UsingDirective(IdentifierName(x)))))
              .AddMembers(NamespaceDeclaration(IdentifierName(Identifier.Namespace)).AddMembers(GetTypeDeclaration()))
              .NormalizeWhitespace();

        [NotNull, ItemNotNull]
        protected virtual ISet<string> GetNamespaces()
        {
            var namespaces = new SortedSet<string>();

            foreach (string ns in Interfaces.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            foreach (string ns in Properties.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            namespaces.Remove(Identifier.Namespace);

            return namespaces;
        }

        [NotNull]
        protected abstract MemberDeclarationSyntax GetTypeDeclaration();
    }
}
