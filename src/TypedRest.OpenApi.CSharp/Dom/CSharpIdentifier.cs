using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpIdentifier
    {
        public static CSharpIdentifier String
            => new CSharpIdentifier("string");

        public static CSharpIdentifier Int
            => new CSharpIdentifier("int");

        [CanBeNull]
        public string Namespace { get; }

        [NotNull]
        public string Name { get; }

        public CSharpIdentifier([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public CSharpIdentifier([CanBeNull] string ns, [NotNull] string name)
        {
            Namespace = ns;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull, ItemNotNull]
        public List<CSharpIdentifier> TypeArguments { get; } = new List<CSharpIdentifier>();

        [NotNull, ItemNotNull]
        public IEnumerable<string> GetNamespaces()
        {
            if (!string.IsNullOrEmpty(Namespace))
                yield return Namespace;

            foreach (string ns in TypeArguments.SelectMany(x => x.GetNamespaces()))
                yield return ns;
        }

        public TypeSyntax ToSyntax()
            => TypeArguments.Count == 0
                ? (TypeSyntax)IdentifierName(Name)
                : GenericName(Identifier(Name)).WithTypeArgumentList(TypeArgumentList(SeparatedList(TypeArguments.Select(x => x.ToSyntax()))));
    }
}
