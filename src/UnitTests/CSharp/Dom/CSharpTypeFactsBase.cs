﻿using FluentAssertions;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public abstract class CSharpTypeFactsBase
    {
        protected static void Assert(CSharpType cSharpClass, string expected)
        {
            string generated = cSharpClass.ToSyntax().ToFullString().Replace("\r\n", "\n");
            generated.Should().Be(expected);
        }
    }
}
