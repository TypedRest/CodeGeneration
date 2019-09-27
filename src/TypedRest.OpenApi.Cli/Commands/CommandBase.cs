using CommandLine;

namespace TypedRest.OpenApi.Cli.Commands
{
    public abstract class CommandBase
    {
        [Option('v', "verbose", HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        public abstract int Run();
    }
}
