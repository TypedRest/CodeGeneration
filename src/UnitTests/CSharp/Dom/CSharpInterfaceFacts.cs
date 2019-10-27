using Xunit;

namespace TypedRest.OpenApi.CSharp.Dom
{
    public class CSharpInterfaceFacts : CSharpTypeFactsBase
    {
        [Fact]
        public void GeneratesCorrectCode()
        {
            var myInterface = new CSharpIdentifier(ns: "Namespace1", name: "MyInterface");
            var baseInterface = new CSharpIdentifier(ns: "Namespace2", name: "BaseInterface");
            var endpointInterface = new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint");

            Assert(new CSharpInterface(myInterface)
            {
                Interfaces = {baseInterface},
                Properties =
                {
                    new CSharpProperty(endpointInterface, "MyProperty")
                }
            }, @"using Namespace2;
using TypedRest.Endpoints;

namespace Namespace1
{
    public interface MyInterface : BaseInterface
    {
        public IEndpoint MyProperty
        {
            get;
        }
    }
}");
        }
    }
}
