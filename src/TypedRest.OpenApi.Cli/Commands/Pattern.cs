using System;
using System.IO;
using System.Text;
using CommandLine;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using TypedRest.OpenApi.Patterns;

namespace TypedRest.OpenApi.Cli.Commands
{
    [Verb("pattern", HelpText = "Finds TypedRest endpoint patterns.")]
    public class Pattern : CommandBase
    {
        [Option('o', "output", HelpText = "The path to the Swagger or OpenAPI spec file to write.", Required = true)]
        public string OutputPath { get; set; } = default!;

        [Option("output-version", HelpText = "The output version. Valid values: 'OpenApi2_0' (Swagger) and 'OpenApi3_0'", Default = OpenApiSpecVersion.OpenApi3_0)]
        public OpenApiSpecVersion OutputVersion { get; set; } = OpenApiSpecVersion.OpenApi3_0;

        [Option("output-format", HelpText = "The output format. Valid values: 'Yaml' and 'Json'", Default = OpenApiFormat.Yaml)]
        public OpenApiFormat OutputFormat { get; set; } = OpenApiFormat.Yaml;

        public override int Run()
        {
            var doc = ReadDoc();

            doc.SetTypedRest(new PatternMatcher().GetEntryEndpoint(doc));

            Write(doc.Serialize(OutputVersion, OutputFormat));

            return 0;
        }

        private void Write(string data)
        {
            if (OutputPath == "-")
                Console.Write(data);
            else
                File.WriteAllText(OutputPath, data, Encoding.UTF8);
        }
    }
}
