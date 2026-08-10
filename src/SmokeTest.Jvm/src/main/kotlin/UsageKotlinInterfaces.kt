package net.typedrest.smoketest

import net.typedrest.smoketest.interfaces.kotlin.ContactElementEndpoint
import net.typedrest.smoketest.interfaces.kotlin.SampleClient
import net.typedrest.smoketest.interfaces.kotlin.SampleClientImpl
import net.typedrest.smoketest.interfaces.kotlin.dtos.Contact
import net.typedrest.smoketest.interfaces.kotlin.dtos.Note
import java.net.URI

// References the client generated with --generate-interfaces. Everything past the constructor is typed as the
// generated interface, which is the point of generating them: nothing here names an implementation.

fun interfaceClient(uri: URI): SampleClient =
    SampleClientImpl(uri)

fun readAllContactsVia(client: SampleClient): List<Contact> =
    client.contacts.readAll()

fun contactVia(client: SampleClient, id: String): ContactElementEndpoint =
    client.contacts[id]

fun readNoteVia(client: SampleClient, id: String): Note =
    client.contacts[id].note.read()

fun pokeVia(client: SampleClient, contact: Contact) {
    client.contacts[contact].poke.invoke()
}
