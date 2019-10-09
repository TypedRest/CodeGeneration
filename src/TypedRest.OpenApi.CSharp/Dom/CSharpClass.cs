using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClass
    {
        [NotNull]
        public CSharpIdentifier Name { get; }

        public CSharpClass([NotNull] CSharpIdentifier name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [CanBeNull]
        public CSharpClassConstruction BaseClass { get; set; }

        [NotNull, ItemNotNull]
        public List<CSharpIdentifier> Interfaces { get; } = new List<CSharpIdentifier>();

        [CanBeNull]
        public string Description { get; set; }

        [NotNull, ItemNotNull]
        public List<CSharpProperty> Properties { get; } = new List<CSharpProperty>();

        [NotNull, ItemNotNull]
        private IEnumerable<string> GetNamespaces()
        {
            var namespaces = new SortedSet<string>();

            if (BaseClass != null)
            {
                foreach (string ns in BaseClass.GetNamespaces())
                    namespaces.Add(ns);
            }

            foreach (var type in Interfaces)
            {
                if (!string.IsNullOrEmpty(type.Namespace))
                    namespaces.Add(type.Namespace);
            }

            foreach (var property in Properties)
            {
                foreach (string ns in property.GetNamespaces())
                    namespaces.Add(ns);
            }

            namespaces.Remove(Name.Namespace);

            return namespaces;
        }

        [NotNull]
        public CompilationUnitSyntax ToSyntax()
        {
            var namespaceDeclaration =
                NamespaceDeclaration(IdentifierName(Name.Namespace))
                   .WithMembers(SingletonList<MemberDeclarationSyntax>(GetClassDeclaration()));

            return CompilationUnit()
                  .WithUsings(List(GetNamespaces().Select(x => UsingDirective(IdentifierName(x)))))
                  .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclaration))
                  .NormalizeWhitespace();
        }

        private ClassDeclarationSyntax GetClassDeclaration()
        {
            var classDeclaration =
                ClassDeclaration(Name.Name)
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithMembers(List(GetMemberDeclarations()));

            var baseTypes = new List<string>();
            if (BaseClass != null) baseTypes.Add(BaseClass.Type.Name);
            baseTypes.AddRange(Interfaces.Select(x => x.Name));

            return baseTypes.Count == 0
                ? classDeclaration
                : classDeclaration.WithBaseList(BaseList(SeparatedList<BaseTypeSyntax>(
                    baseTypes.Select(x => SimpleBaseType(IdentifierName(x))))));
        }

        [NotNull, ItemNotNull]
        private IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations()
        {
            if (BaseClass != null && BaseClass.Parameters.Count != 0)
                yield return BaseClass.ToConstructorSyntax(Name.Name);

            foreach (var property in Properties)
                yield return property.ToSyntax();
        }
    }
}
