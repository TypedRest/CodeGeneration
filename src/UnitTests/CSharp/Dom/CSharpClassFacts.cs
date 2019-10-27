using Xunit;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpClassFacts : CSharpTypeFactsBase
    {
        [Fact]
        public void GeneratesCorrectCode()
        {
            var myClass = new CSharpIdentifier(ns: "Namespace1", name: "MyClass");
            var myModel = new CSharpIdentifier(ns: "Models", name: "MyModel");
            var myInterface = new CSharpIdentifier(ns: "Namespace1", name: "MyInterface") {TypeArguments = {myModel}};
            var otherClass = new CSharpIdentifier(ns: "Namespace1", name: "OtherClass") {TypeArguments = {myModel}};
            var baseClass = new CSharpIdentifier(ns: "Namespace2", name: "BaseClass");
            var endpointInterface = new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint");

            Assert(new CSharpClass(myClass)
            {
                Description = "My class",
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
                    new CSharpProperty(myInterface, "MyProperty")
                    {
                        Description = "My property",
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
    /// <summary>
    /// My class
    /// </summary>
    public class MyClass : BaseClass, MyInterface<MyModel>
    {
        public MyClass(IEndpoint referrer): base(referrer, relativeUri: ""./sample"")
        {
        }

        /// <summary>
        /// My property
        /// </summary>
        public MyInterface<MyModel> MyProperty => new OtherClass<MyModel>(arg1: ""value"");
    }
}");
        }
    }
}
