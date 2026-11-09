using SmokeTest.CSharp.NestedClient;

namespace SmokeTest.CSharp;

public static class NestedUsage
{
    public static INestedClient Client(Uri uri)
        => new NestedClient.NestedClient(uri);

    public static ITeamElementEndpoint GetTeam(INestedClient client, string id)
        => client.Teams[id];

    public static ITeamElementEndpoint GetTeam(INestedClient client, Team team)
        => client.Teams[team];

    public static async Task<ITeamElementEndpoint?> CreateTeam(INestedClient client, Team team, CancellationToken cancellationToken = default)
        => await client.Teams.CreateAsync(team, cancellationToken);

    public static IProjectElementEndpoint GetProject(INestedClient client, string id)
        => client.Projects[id];

    // Generated for System.Text.Json, so the enum has to carry its own string converter
    public static string SerializeStatus(ProjectStatus status)
        => System.Text.Json.JsonSerializer.Serialize(status);

    public static async Task ArchiveTeam(ITeamElementEndpoint team, CancellationToken cancellationToken = default)
        => await team.Archive.InvokeAsync(cancellationToken);
}
