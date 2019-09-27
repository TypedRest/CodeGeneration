using CommandLine;
using TypedRest.OpenApi.Cli.Commands;

namespace TypedRest.OpenApi.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
            => Parser.Default
                     .ParseArguments<Pattern, Generate>(args)
                     .MapResult(
                          (Pattern command) => command.Run(),
                          (Generate command) => command.Run(),
                          _ => 1);
    }
}
