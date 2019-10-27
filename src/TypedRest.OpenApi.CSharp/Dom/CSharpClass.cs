using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClass : CSharpType
    {
        public override CSharpIdentifier Identifier { get; }

        public CSharpClass([NotNull] CSharpIdentifier identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        [CanBeNull]
        public CSharpClassConstruction BaseClass { get; set; }

        protected override ISet<string> GetNamespaces()
        {
            var namespaces = base.GetNamespaces();

            if (BaseClass != null)
            {
                foreach (string ns in BaseClass.GetNamespaces())
                    namespaces.Add(ns);
            }

            return namespaces;
        }

        protected override MemberDeclarationSyntax GetTypeDeclaration()
            => ClassDeclaration(Identifier.Name)
              .AddModifiers(Token(SyntaxKind.PublicKeyword))
              .WithBaseList(BaseList(SeparatedList(GetBaseTypes())))
              .WithMembers(List(GetMemberDeclarations()));

        [NotNull, ItemNotNull]
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
                yield return BaseClass.ToConstructorSyntax(Identifier.Name);

            foreach (var property in Properties)
                yield return property.ToSyntax();
        }
    }
}
