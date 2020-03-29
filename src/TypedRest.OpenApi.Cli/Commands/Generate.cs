using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using TypedRest.OpenApi.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.OpenApi.Cli.Commands
{
    [Verb("generate", HelpText = "Generates a TypedRest client.")]
    public class Generate : CommandBase
    {
        [Option('o', "output", HelpText = "The directory to write the generated source code to.", Required = true)]
        public string OutputDir { get; set; } = default!;

        [Option('s', "service-name", HelpText = "The service name to use for the entry endpoint.", Required = true)]
        public string ServiceName { get; set; } = default!;

        [Option('n', "namespace", HelpText = "The C# namespace for the endpoint. Uses service-name if not set.")]
        public string? Namespace { get; set; }

        [Option("dto-namespace", HelpText = "The C# namespace for the DTOs. Uses namespace if not set.")]
        public string? DtoNamespace { get; set; }

        [Option("generate-interfaces", HelpText = "Controls whether to generate interfaces for endpoints.", Default = true)]
        public bool GenerateInterfaces { get; set; } = true;

        [Option("generate-dtos", HelpText = "Controls whether to generate DTOs.", Default = true)]
        public bool GenerateDtos { get; set; } = true;

        public override int Run()
        {
            var doc = ReadDoc();
            var naming = new NamingStrategy(ServiceName, Namespace ?? ServiceName, DtoNamespace ?? Namespace ?? ServiceName);

            var types = doc.GenerateTypedRestEndpoints(naming, GenerateInterfaces);
            if (GenerateDtos)
                types = types.Concat(doc.GenerateDtos(naming));

            WriteSource(types);

            return 0;
        }

        private void WriteSource(IEnumerable<ICSharpType> types)
        {
            Directory.CreateDirectory(OutputDir);
            foreach (var type in types)
                type.WriteToDirectory(OutputDir);
        }
    }
}
