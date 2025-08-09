namespace TaskManagement.Application.Common
{
    /// <summary>
    /// Handler interface for commands.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
