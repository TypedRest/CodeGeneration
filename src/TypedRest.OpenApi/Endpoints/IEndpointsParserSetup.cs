namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Fluent interface for configuring an <see cref="EndpointsParser"/>.
    /// </summary>
    public interface IEndpointsParserSetup
    {
        /// <summary>
        /// Adds <typeparamref name="T"/> to the list of known endpoint types.
        /// </summary>
        IEndpointsParserSetup Add<T>()
            where T : IEndpoint, new();
    }
}
