import {SampleClient} from "./generated/sample";
import {Contact} from "./generated/sample/dtos/Contact";
import {Note} from "./generated/sample/dtos/Note";

// References the generated client, so that the compiler has to resolve the generated types and check that they
// line up with the real typedrest package.

export function client(uri: string): SampleClient {
  return new SampleClient(uri);
}

export async function readAllContacts(client: SampleClient): Promise<Contact[]> {
  return await client.contacts.readAll();
}

export async function readNote(client: SampleClient, id: string): Promise<Note> {
  return await client.contacts.get(id).note.read();
}

export async function writeNote(client: SampleClient, id: string, note: Note): Promise<void> {
  await client.contacts.get(id).note.set(note);
}

export async function pokeAndDownload(client: SampleClient, contact: Contact): Promise<Blob> {
  // get() also accepts an entity, extracting its id property
  const element = client.contacts.get(contact);
  await element.poke.invoke();
  return await element.picture.download();
}

export async function createContact(client: SampleClient, contact: Contact): Promise<Note | undefined> {
  const created = await client.contacts.create(contact);
  return await created?.note.read();
}

export function requiredProperties(): Contact {
  // firstName and lastName are required, id is not
  return {firstName: "John", lastName: "Doe"};
}
