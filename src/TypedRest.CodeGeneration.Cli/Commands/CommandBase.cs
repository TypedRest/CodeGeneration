using System;
using System.IO;
using CommandLine;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace TypedRest.CodeGeneration.Cli.Commands
{
    public abstract class CommandBase
    {
        public abstract int Run();

        [Option('f', "file", HelpText = "The path to the Swagger or OpenAPI spec file.", Required = true)]
        public string InputPath { get; set; } = default!;

        protected (OpenApiDocument, OpenApiSpecVersion) ReadDoc()
        {
            var doc = OpenApiDocumentExtensions.ReadWithTypedRest(ReadFile(), out var diagnostic);
            foreach (var error in diagnostic.Errors)
                Console.Error.WriteLine("Warning: " + error.Message);
            return (doc, diagnostic.SpecificationVersion);
        }

        private string ReadFile()
            => InputPath == "-"
                ? Console.In.ReadToEnd()
                : File.ReadAllText(InputPath);
    }
}
