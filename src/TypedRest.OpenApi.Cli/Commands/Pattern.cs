using CommandLine;

namespace TypedRest.OpenApi.Cli.Commands
{
    [Verb("pattern", HelpText = "Finds TypedRest endpoint patterns.")]
    public class Pattern : CommandBase
    {
        public override int Run()
        {
            return 0;
        }
    }
}
