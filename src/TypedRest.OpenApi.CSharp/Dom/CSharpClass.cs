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

            foreach (string ns in Interfaces.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

            foreach (string ns in Properties.SelectMany(x => x.GetNamespaces()))
                namespaces.Add(ns);

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
            => ClassDeclaration(Name.Name)
              .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
              .WithBaseList(BaseList(SeparatedList(GetBaseTypes())))
              .WithMembers(List(GetMemberDeclarations()));

        private IEnumerable<BaseTypeSyntax> GetBaseTypes()
        {
            if (BaseClass != null)
                yield return SimpleBaseType(BaseClass.Type.ToSyntax());

            foreach (var @interface in Interfaces)
                yield return SimpleBaseType(@interface.ToSyntax());
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
