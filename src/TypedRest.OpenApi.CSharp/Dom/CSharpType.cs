using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public abstract class CSharpType : ICSharpType
    {
        public abstract CSharpIdentifier Identifier { get; }

        public string? Description { get; set; }

        public List<CSharpAttribute> Attributes { get; } = new List<CSharpAttribute>();

        public List<CSharpIdentifier> Interfaces { get; } = new List<CSharpIdentifier>();

        public List<CSharpProperty> Properties { get; } = new List<CSharpProperty>();

        public CompilationUnitSyntax ToSyntax()
        {
            var namespaces = GetNamespaces();
            if (!string.IsNullOrEmpty(Identifier.Namespace))
                namespaces.Remove(Identifier.Namespace);

            return CompilationUnit()
                  .WithUsings(List(namespaces.Select(x => UsingDirective(IdentifierName(x)))))
                  .AddMembers(NamespaceDeclaration(IdentifierName(Identifier.Namespace)).AddMembers(GetTypeDeclaration()))
                  .NormalizeWhitespace();
        }

        protected virtual ISet<string> GetNamespaces()
        {
            var namespaces = new SortedSet<string>();

            foreach (string? ns in Attributes.Select(x => x.Identifier.Namespace))
            {
                if (ns != null)
                    namespaces.Add(ns);
            }

            foreach (string ns in Interfaces.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            foreach (string ns in Properties.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            return namespaces;
        }

        protected abstract MemberDeclarationSyntax GetTypeDeclaration();

        public override string ToString() => Identifier.ToString();
    }
}
