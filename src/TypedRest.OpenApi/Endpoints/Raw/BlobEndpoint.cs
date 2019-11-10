namespace TypedRest.OpenApi.Endpoints.Raw
{
    /// <summary>
    /// Endpoint for a binary blob that can be downloaded or uploaded.
    /// </summary>
    public class BlobEndpoint : Endpoint
    {
        public override string Type => "blob";
    }
}
