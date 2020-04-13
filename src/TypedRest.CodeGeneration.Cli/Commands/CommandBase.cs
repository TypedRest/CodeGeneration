using System;
using System.IO;
using CommandLine;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace TypedRest.CodeGeneration.Cli.Commands
{
    public abstract class CommandBase
    {
        public abstract int Run();

        [Option('f', "file", HelpText = "The path to the Swagger or OpenAPI spec file.", Required = true)]
        public string InputPath { get; set; } = default!;

        protected (OpenApiDocument, OpenApiSpecVersion) ReadDoc()
        {
            var reader = new OpenApiStreamReader(new OpenApiReaderSettings().AddTypedRest());

            using var stream = InputPath == "-"
                ? Console.OpenStandardInput()
                : File.OpenRead(InputPath);
            var doc = reader.Read(stream, out var diagnostic);

            foreach (var error in diagnostic.Errors)
                Console.Error.WriteLine("Warning: " + error.Message);

            return (doc, diagnostic.SpecificationVersion);
        }
    }
}
