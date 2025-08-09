namespace TaskManagement.Application.Common
{
    /// <summary>
    /// Marker interface for queries with a response type.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    public interface IQuery<TResponse> : IQuery
    {
    }
}
