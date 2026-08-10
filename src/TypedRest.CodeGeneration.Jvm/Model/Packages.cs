namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// The packages generated code imports from.
/// </summary>
public static class Packages
{
    /// <summary>Holds <c>String</c> and <c>Object</c>. Never imported, but needed to qualify them.</summary>
    public static JvmPackage JavaLang { get; } = JvmPackage.External("java.lang");

    /// <summary>Holds <c>List</c>, <c>Map</c> and <c>UUID</c>.</summary>
    public static JvmPackage JavaUtil { get; } = JvmPackage.External("java.util");

    /// <summary>Holds <c>URI</c>, which every endpoint constructor takes.</summary>
    public static JvmPackage JavaNet { get; } = JvmPackage.External("java.net");

    /// <summary>Holds <c>InputStream</c>, used for blob and upload endpoints.</summary>
    public static JvmPackage JavaIo { get; } = JvmPackage.External("java.io");

    /// <summary>Holds <c>OffsetDateTime</c> and <c>LocalDate</c>.</summary>
    public static JvmPackage JavaTime { get; } = JvmPackage.External("java.time");

    /// <summary>Holds <c>Endpoint</c> and <c>EntryEndpoint</c>.</summary>
    public static JvmPackage Endpoints { get; } = JvmPackage.External("net.typedrest.endpoints");

    /// <summary>Holds <c>ElementEndpoint</c>, <c>CollectionEndpoint</c>, <c>GenericCollectionEndpoint</c>, <c>IndexerEndpoint</c> and their <c>Impl</c> classes.</summary>
    public static JvmPackage Generic { get; } = JvmPackage.External("net.typedrest.endpoints.generic");

    /// <summary>Holds <c>ActionEndpoint</c>, <c>ProducerEndpoint</c>, <c>ConsumerEndpoint</c>, <c>FunctionEndpoint</c> and their <c>Impl</c> classes.</summary>
    public static JvmPackage Rpc { get; } = JvmPackage.External("net.typedrest.endpoints.rpc");

    /// <summary>Holds <c>BlobEndpoint</c>, <c>UploadEndpoint</c> and their <c>Impl</c> classes.</summary>
    public static JvmPackage Raw { get; } = JvmPackage.External("net.typedrest.endpoints.raw");

    /// <summary>Holds the streaming and polling endpoints, from the separate <c>typedrest-reactive</c> artifact.</summary>
    public static JvmPackage Reactive { get; } = JvmPackage.External("net.typedrest.endpoints.reactive");

    /// <summary>Holds the <c>Serializer</c> implementations the entry endpoint is constructed with.</summary>
    public static JvmPackage Serializers { get; } = JvmPackage.External("net.typedrest.serializers");

    /// <summary>Holds <c>HttpCredentials</c>.</summary>
    public static JvmPackage Http { get; } = JvmPackage.External("net.typedrest.http");

    /// <summary>
    /// The optional credentials parameter of the entry endpoint constructor.
    /// </summary>
    public static JvmIdentifier HttpCredentials { get; } = new(Http, "HttpCredentials");

    /// <summary>
    /// The base type of every generated entry endpoint.
    /// </summary>
    public static JvmIdentifier EntryEndpoint { get; } = new JvmIdentifier(Endpoints, "EntryEndpoint").Implementing("Endpoint");

    /// <summary>
    /// The type every endpoint constructor takes as its <c>referrer</c>.
    /// </summary>
    public static JvmIdentifier Endpoint { get; } = new(Endpoints, "Endpoint");

    /// <summary>
    /// The base of an endpoint that has no more specific kind.
    /// </summary>
    public static JvmIdentifier AbstractEndpoint { get; } = new JvmIdentifier(Endpoints, "AbstractEndpoint").Implementing("Endpoint");

    /// <summary>
    /// Returns the implementation class a generated endpoint derives from, paired with the interface behind it.
    /// </summary>
    public static JvmIdentifier Implementation(JvmPackage package, string name)
        => new JvmIdentifier(package, name + "Impl").Implementing(name);
}
