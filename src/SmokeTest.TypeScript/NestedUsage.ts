import {Endpoint} from "typedrest/endpoints";
import {NestedSampleClient} from "./generated/nested-sample";
import {TeamElementEndpoint} from "./generated/nested-sample/TeamElementEndpoint";
import {ProjectElementEndpoint} from "./generated/nested-sample/ProjectElementEndpoint";
import {Team} from "./generated/nested-sample/dtos/Team";

// Collection and indexer endpoints that have children of their own, so that classes are generated for them
// rather than the built-in ones being used directly. Their element endpoints are then passed to the base
// constructor as a factory, which only type-checks if the generated element classes keep accepting a URI.

// An element class must keep accepting a URI: the collection or indexer that owns it hands it a different one
// per element. Baking a fixed URI in would still satisfy the factory parameter, because TypeScript accepts a
// constructor with fewer parameters where one with more is expected, but every element would then silently
// resolve to the same URI at runtime. Comparing the constructor parameters directly is what catches that.
type ElementConstructorParameters = [referrer: Endpoint, relativeUri: URL | string];
const _teamTakesUri: ElementConstructorParameters = null! as ConstructorParameters<typeof TeamElementEndpoint>;
const _projectTakesUri: ElementConstructorParameters = null! as ConstructorParameters<typeof ProjectElementEndpoint>;
void _teamTakesUri;
void _projectTakesUri;

export async function syncAndReadTeams(client: NestedSampleClient): Promise<Team[]> {
  await client.teams.sync.invoke();
  return await client.teams.readAll();
}

export function teamElement(client: NestedSampleClient, id: string): TeamElementEndpoint {
  // The generated element class, not the built-in ElementEndpoint
  return client.teams.get(id);
}

export async function archiveTeam(client: NestedSampleClient, team: Team): Promise<void> {
  await client.teams.get(team).archive.invoke();
}

export async function createTeam(client: NestedSampleClient, team: Team): Promise<TeamElementEndpoint | undefined> {
  return await client.teams.create(team);
}

export function projectElement(client: NestedSampleClient, id: string): ProjectElementEndpoint {
  return client.projects.get(id);
}

export async function searchAndRename(client: NestedSampleClient, id: string): Promise<void> {
  await client.projects.search.invoke();
  await client.projects.get(id).rename.invoke();
}
