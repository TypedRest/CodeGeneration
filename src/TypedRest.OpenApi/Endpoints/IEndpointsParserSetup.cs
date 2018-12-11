using JetBrains.Annotations;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Fluent interface for configuring an <see cref="EndpointsParser"/>.
    /// </summary>
    [PublicAPI]
    public interface IEndpointsParserSetup
    {
        /// <summary>
        /// Adds <typeparamref name="T"/> to the list of known endpoint types.
        /// </summary>
        [NotNull]
        IEndpointsParserSetup Add<T>()
            where T : IEndpoint, new();
    }
}
