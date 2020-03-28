using System;
using System.IO;
using CommandLine;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace TypedRest.OpenApi.Cli.Commands
{
    public abstract class CommandBase
    {
        public abstract int Run();

        [Option('v', "verbose", HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('f', "file", HelpText = "The path to the Swagger or OpenAPI spec file.", Required = true)]
        public string InputPath { get; set; } = default!;

        protected OpenApiDocument ReadDoc()
            => OpenApiDocumentExtensions.ReadWithTypedRest(ReadFile());

        private string ReadFile()
            => InputPath == "-"
                ? Console.In.ReadToEnd()
                : File.ReadAllText(InputPath);
    }
}
