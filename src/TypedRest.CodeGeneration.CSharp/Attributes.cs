using System.Reflection;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp;

internal static class Attributes
{
    /// <summary>
    /// The version of the code generator, without build metadata (e.g. a commit hash).
    /// </summary>
    private static readonly string Version = GetVersion();

    private static string GetVersion()
    {
        var assembly = typeof(Attributes).Assembly;
        string? version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                       ?? assembly.GetName().Version?.ToString();
        if (string.IsNullOrEmpty(version)) return "0.0.0";

        int metadataStart = version!.IndexOf('+');
        return metadataStart < 0 ? version : version.Substring(0, metadataStart);
    }

    public static CSharpAttribute GeneratedCode
        => new(new CSharpIdentifier("System.CodeDom.Compiler", "GeneratedCodeAttribute"))
        {
            Arguments = {"TypedRest.CodeGeneration", Version}
        };

    public static CSharpAttribute Obsolete
        => new(new CSharpIdentifier("System", "ObsoleteAttribute"));

    public static CSharpAttribute Required
        => new(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "RequiredAttribute"));

    public static CSharpAttribute Key
        => new(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "KeyAttribute"));
}
