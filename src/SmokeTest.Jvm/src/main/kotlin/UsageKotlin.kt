package net.typedrest.smoketest

import net.typedrest.smoketest.kotlin.SampleClient
import net.typedrest.smoketest.kotlin.dtos.Contact
import net.typedrest.smoketest.kotlin.dtos.Note
import java.io.InputStream
import java.net.URI

fun client(uri: URI): SampleClient =
    SampleClient(uri)

fun readAllContacts(client: SampleClient): List<Contact> =
    client.contacts.readAll()

fun readNote(client: SampleClient, id: String): Note =
    client.contacts[id].note.read()

fun writeNote(client: SampleClient, id: String, note: Note) {
    client.contacts[id].note.set(note)
}

fun pokeAndDownload(client: SampleClient, contact: Contact): InputStream {
    // get() also accepts an entity, extracting its id property
    val element = client.contacts[contact]
    element.poke.invoke()
    return element.picture.download()
}

fun createContact(client: SampleClient, contact: Contact): Note? =
    client.contacts.create(contact)?.note?.read()

fun requiredProperties(): Contact =
    // firstName and lastName are required, id is not and defaults to null
    Contact(firstName = "John", lastName = "Doe")
