package net.typedrest.smoketest;

import net.typedrest.smoketest.java.SampleClient;
import net.typedrest.smoketest.java.dtos.Contact;
import net.typedrest.smoketest.java.dtos.Note;

import java.io.InputStream;
import java.net.URI;
import java.util.List;

public final class UsageJava {
    private UsageJava() {}

    public static SampleClient client(URI uri) {
        return new SampleClient(uri);
    }

    public static List<Contact> readAllContacts(SampleClient client) {
        return client.contacts.readAll();
    }

    public static Note readNote(SampleClient client, String id) {
        return client.contacts.get(id).note.read();
    }

    public static void writeNote(SampleClient client, String id, Note note) {
        client.contacts.get(id).note.set(note);
    }

    public static InputStream pokeAndDownload(SampleClient client, Contact contact) {
        // get() also accepts an entity, extracting its id property
        var element = client.contacts.get(contact);
        element.poke.invoke();
        return element.picture.download();
    }

    public static Note createContact(SampleClient client, Contact contact) {
        var created = client.contacts.create(contact);
        return created == null ? null : created.note.read();
    }

    public static Contact requiredProperties() {
        // The generated DTO has both a no-argument constructor for the serializer and a full one
        return new Contact(null, "John", "Doe");
    }
}
