﻿using System.IO;
using System.Text;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public static class CSharpTypeExtensions
    {
        public static void WriteToFile(this ICSharpType type, string path)
            => File.WriteAllText(path, type.ToSyntax().ToFullString(), Encoding.UTF8);

        public static void WriteToDirectory(this ICSharpType type, string path)
            => type.WriteToFile(Path.Combine(path, type.Identifier.Name + ".cs"));
    }
}