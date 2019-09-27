using CommandLine;

namespace TypedRest.OpenApi.Cli.Commands
{
    [Verb("generate", HelpText = "Generates a TypedRest client.")]
    public class Generate : CommandBase
    {
        public override int Run()
        {
            return 0;
        }
    }
}
