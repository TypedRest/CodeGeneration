using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp;

internal static class Attributes
{
    public static CSharpAttribute GeneratedCode
        => new(new CSharpIdentifier("System.CodeDom.Compiler", "GeneratedCodeAttribute"))
        {
            Arguments = {"TypedRest.CodeGeneration", "1.0.0"}
        };

    public static CSharpAttribute Obsolete
        => new(new CSharpIdentifier("System", "ObsoleteAttribute"));

    public static CSharpAttribute Required
        => new(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "RequiredAttribute"));

    public static CSharpAttribute Key
        => new(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "KeyAttribute"));
}
