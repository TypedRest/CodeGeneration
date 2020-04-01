using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp
{
    public static class Attributes
    {
        public static CSharpAttribute GeneratedCode
            => new CSharpAttribute(new CSharpIdentifier("System.CodeDom.Compiler", "GeneratedCode"))
            {
                Arguments = {"TypedRest.CodeGeneration", "1.0.0"}
            };

        public static CSharpAttribute JsonProperty(string name)
            => new CSharpAttribute(new CSharpIdentifier("Newtonsoft.Json", "JsonProperty"))
            {
                Arguments = {name}
            };

        public static CSharpAttribute Required
            => new CSharpAttribute(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "Required"));

        public static CSharpAttribute Key
            => new CSharpAttribute(new CSharpIdentifier("System.ComponentModel.DataAnnotations", "Key"));
    }
}
