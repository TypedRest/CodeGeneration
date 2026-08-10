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
        return client.getContacts().readAll();
    }

    public static Note readNote(SampleClient client, String id) {
        return client.getContacts().get(id).getNote().read();
    }

    public static void writeNote(SampleClient client, String id, Note note) {
        client.getContacts().get(id).getNote().set(note);
    }

    public static InputStream pokeAndDownload(SampleClient client, Contact contact) {
        // get() also accepts an entity, extracting its id property
        var element = client.getContacts().get(contact);
        element.getPoke().invoke();
        return element.getPicture().download();
    }

    public static Note createContact(SampleClient client, Contact contact) {
        var created = client.getContacts().create(contact);
        return created == null ? null : created.getNote().read();
    }

    public static Contact requiredProperties() {
        // The generated DTO has both a no-argument constructor for the serializer and a full one
        return new Contact(null, "John", "Doe");
    }
}
