using Xunit;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClassFacts : CSharpTypeFactsBase
    {
        [Fact]
        public void GeneratesCorrectCode()
        {
            var myClass = new CSharpIdentifier(ns: "Namespace1", name: "MyClass");
            var myInterface = new CSharpIdentifier(ns: "Namespace1", name: "MyInterface");
            var otherClass = new CSharpIdentifier(ns: "Namespace1", name: "OtherClass");
            var baseClass = new CSharpIdentifier(ns: "Namespace2", name: "BaseClass") {TypeArguments = {new CSharpIdentifier(ns: "Models", name: "MyModel")}};
            var endpointInterface = new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint");

            Assert(new CSharpClass(myClass)
            {
                BaseClass = new CSharpClassConstruction(baseClass)
                {
                    Parameters =
                    {
                        new CSharpParameter(endpointInterface, "referrer"),
                        new CSharpParameter(CSharpIdentifier.String, "relativeUri", "./sample")
                    }
                },
                Interfaces = {myInterface},
                Properties =
                {
                    new CSharpProperty(otherClass, "MyProperty")
                    {
                        GetterExpression = new CSharpClassConstruction(otherClass)
                        {
                            Parameters =
                            {
                                new CSharpParameter(CSharpIdentifier.String, "arg1", "value")
                            }
                        }
                    }
                }
            }, @"using Models;
using Namespace2;
using TypedRest.Endpoints;

namespace Namespace1
{
    public class MyClass : BaseClass<MyModel>, MyInterface
    {
        public MyClass(IEndpoint referrer): base(referrer, relativeUri: ""./sample"")
        {
        }

        public OtherClass MyProperty => new OtherClass(arg1: ""value"");
    }
}");
        }
    }
}
