using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp
{
    internal static class Attributes
    {
        public static CSharpAttribute GeneratedCode
            => new CSharpAttribute(new CSharpIdentifier("System.CodeDom.Compiler", "GeneratedCodeAttribute"))
            {
                Arguments = {"TypedRest.CodeGeneration", "1.0.0"}
            };

        public static CSharpAttribute JsonProperty(string name)
            => new CSharpAttribute(new CSharpIdentifier("Newtonsoft.Json", "JsonPropertyAttribute"))
            {
                Arguments = {name}
            };

        public static CSharpAttribute EnumMember(string name)
            => new CSharpAttribute(new CSharpIdentifier("System.Runtime.Serialization", "EnumMemberAttribute"))
            {
                NamedArguments =
                {
                    ("Value", name)
                }
            };

        public static CSharpAttribute Required
            => new CSharpAttribute(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "RequiredAttribute"));

        public static CSharpAttribute Key
            => new CSharpAttribute(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "KeyAttribute"));
    }
}
