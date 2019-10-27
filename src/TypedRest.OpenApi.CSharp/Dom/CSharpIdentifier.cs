using System;
using System.Collections.Generic;
using JetBrains.Annotations;

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

        public CSharpIdentifier([NotNull] string ns, [NotNull] string name)
        {
            Namespace = ns ?? throw new ArgumentNullException(nameof(ns));;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull, ItemNotNull]
        public List<string> TypeArguments { get; } = new List<string>();
    }
}
