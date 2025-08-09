namespace TaskManagement.Application.Common
{
    /// <summary>
    /// Marker interface for commands with a response type.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    public interface ICommand<TResponse> : ICommand
    {
    }
}
