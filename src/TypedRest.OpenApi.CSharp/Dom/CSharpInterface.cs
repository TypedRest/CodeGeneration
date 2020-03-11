﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpInterface : CSharpType
    {
        public override CSharpIdentifier Identifier { get; }

        public CSharpInterface(CSharpIdentifier identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        protected override MemberDeclarationSyntax GetTypeDeclaration()
            => InterfaceDeclaration(Identifier.Name)
              .AddModifiers(Token(SyntaxKind.PublicKeyword))
              .AddAttributeLists(GeneratedCodeAttribute)
              .WithDocumentation(Description)
              .WithBaseList(BaseList(SeparatedList(GetBaseTypes())))
              .WithMembers(List(GetMemberDeclarations()));

        private IEnumerable<BaseTypeSyntax> GetBaseTypes()
            => Interfaces.Select(x => SimpleBaseType(x.ToSyntax()));

        private IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations()
            => Properties.Select(property => property.ToSyntax(publicKeyword: false));
    }
}
