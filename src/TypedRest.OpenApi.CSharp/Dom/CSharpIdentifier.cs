using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpIdentifier
    {
        public static CSharpIdentifier Bool => new CSharpIdentifier("bool");
        public static CSharpIdentifier Int => new CSharpIdentifier("int");
        public static CSharpIdentifier Long => new CSharpIdentifier("long");
        public static CSharpIdentifier Float => new CSharpIdentifier("float");
        public static CSharpIdentifier Double => new CSharpIdentifier("double");
        public static CSharpIdentifier String => new CSharpIdentifier("string");
        public static CSharpIdentifier Uri => new CSharpIdentifier("System", "Uri");

        public string? Namespace { get; }

        public string Name { get; }

        public CSharpIdentifier(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public CSharpIdentifier(string? ns, string name)
        {
            Namespace = ns;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public List<CSharpIdentifier> TypeArguments { get; } = new List<CSharpIdentifier>();

        public IEnumerable<string> GetNamespaces()
        {
            if (!string.IsNullOrEmpty(Namespace))
                yield return Namespace;

            foreach (string ns in TypeArguments.SelectMany(x => x.GetNamespaces()))
                yield return ns;
        }

        public TypeSyntax ToSyntax()
            => Name switch
            {
                "bool" => PredefinedType(Token(SyntaxKind.BoolKeyword)),
                "int" => PredefinedType(Token(SyntaxKind.IntKeyword)),
                "long" => PredefinedType(Token(SyntaxKind.LongKeyword)),
                "float" => PredefinedType(Token(SyntaxKind.FloatKeyword)),
                "double" => PredefinedType(Token(SyntaxKind.DoubleKeyword)),
                "string" => PredefinedType(Token(SyntaxKind.StringKeyword)),
                _ => (TypeArguments.Count == 0 ? (TypeSyntax)IdentifierName(Name) : GenericName(Identifier(Name)).WithTypeArgumentList(TypeArgumentList(SeparatedList(TypeArguments.Select(x => x.ToSyntax())))))
            };

        public CSharpIdentifier ToInterface()
        {
            var result = new CSharpIdentifier(Namespace, "I" + Name);
            result.TypeArguments.AddRange(TypeArguments);
            return result;
        }

        public override string ToString()
            => TypeArguments.Count == 0
                ? Name
                : Name + "<" + string.Join(", ", TypeArguments) + ">";
    }
}
