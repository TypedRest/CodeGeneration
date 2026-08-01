using SmokeTest.Client;

namespace SmokeTest;

/// <summary>
/// References the generated client, so that the compiler has to resolve the generated types.
/// </summary>
public static class Usage
{
    public static ISampleClient Client(Uri uri)
        => new SampleClient(uri);

    public static async Task<Contact> GetFirstContact(ISampleClient client, CancellationToken cancellationToken = default)
        => (await client.Contacts.ReadAllAsync(cancellationToken))[0];

    public static async Task<Note> GetNote(IContactElementEndpoint contact, CancellationToken cancellationToken = default)
        => await contact.Note.ReadAsync(cancellationToken);
}
