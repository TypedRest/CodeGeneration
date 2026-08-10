package net.typedrest.smoketest;

import net.typedrest.smoketest.interfaces.java.ContactElementEndpoint;
import net.typedrest.smoketest.interfaces.java.SampleClient;
import net.typedrest.smoketest.interfaces.java.SampleClientImpl;
import net.typedrest.smoketest.interfaces.java.dtos.Contact;
import net.typedrest.smoketest.interfaces.java.dtos.Note;

import java.net.URI;
import java.util.List;

// References the client generated with --generate-interfaces. A Java interface cannot declare fields, so the child
// endpoints are accessor methods here rather than the public final fields the interface-less variant exposes.

public final class UsageJavaInterfaces {
    private UsageJavaInterfaces() {}

    public static SampleClient interfaceClient(URI uri) {
        return new SampleClientImpl(uri);
    }

    public static List<Contact> readAllContactsVia(SampleClient client) {
        return client.getContacts().readAll();
    }

    public static ContactElementEndpoint contactVia(SampleClient client, String id) {
        return client.getContacts().get(id);
    }

    public static Note readNoteVia(SampleClient client, String id) {
        return client.getContacts().get(id).getNote().read();
    }

    public static void pokeVia(SampleClient client, Contact contact) {
        client.getContacts().get(contact).getPoke().invoke();
    }
}
