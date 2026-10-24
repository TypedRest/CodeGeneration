import {ReactiveSampleClient} from "./generated/reactive-sample";
import {Contact} from "./generated/reactive-sample/dtos/Contact";
import {ContactEvent} from "./generated/reactive-sample/dtos/ContactEvent";
import {Note} from "./generated/reactive-sample/dtos/Note";

// Every reactive kind, so that the compiler checks the generated getters against the real typedrest package's
// reactive endpoints, including their stream() signatures.

export async function watchContact(client: ReactiveSampleClient, id: string, signal: AbortSignal): Promise<ContactEvent[]> {
  const events: ContactEvent[] = [];
  for await (const x of client.contacts.get(id).events.stream(signal)) events.push(x);
  return events;
}

export async function pollNote(client: ReactiveSampleClient, id: string): Promise<Note | undefined> {
  for await (const note of client.contacts.get(id).note.stream()) return note;
  return undefined;
}

export async function pokeAndStreamContacts(client: ReactiveSampleClient, id: string): Promise<Contact[]> {
  await client.contacts.get(id).poke.invoke();
  const contacts: Contact[] = [];
  for await (const x of client.contacts.stream(-10)) contacts.push(x);
  return contacts;
}

export async function pollWatched(client: ReactiveSampleClient, id: string): Promise<Contact | undefined> {
  // The collection's element is a PollingEndpoint, not a plain ElementEndpoint
  for await (const contact of client.watched.get(id).stream()) return contact;
  return undefined;
}

export async function streamAll(client: ReactiveSampleClient): Promise<ContactEvent[]> {
  const events: ContactEvent[] = [];
  for await (const x of client.changes) events.push(x);
  for await (const x of client.updates.stream()) events.push(x);
  for await (const x of client.log.stream()) events.push(x);
  return events;
}
