namespace OrderQuery.Api.Application.Commands
{
    public interface ICommandResult<TResult>
    {
        string Error { get; }

        TResult Result { get; }

        CommandResultStatus Status { get; }
    }

    public class CommandResult<TResult>
        : ICommandResult<TResult>
    {
        private CommandResult(
            CommandResultStatus commandResultStatus, 
            TResult result, 
            string error)
        {
            this.Status = commandResultStatus;
            this.Result = result;
            this.Error = error;
        }

        public string Error { get; }
        public TResult Result { get; }
        public CommandResultStatus Status { get; }

        public static CommandResult<TResult> Failure(string error)
        {
            return new CommandResult<TResult>(CommandResultStatus.Failure, default(TResult), error);
        }

        public static CommandResult<TResult> Success(TResult result)
        {
            return new CommandResult<TResult>(CommandResultStatus.Success, result, null);
        }
        
    }

    public enum CommandResultStatus
    {
        Failure,
        Success
    }
}
