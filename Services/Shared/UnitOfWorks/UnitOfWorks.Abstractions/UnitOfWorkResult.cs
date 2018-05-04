namespace UnitOfWorks.Abstractions
{
    public class UnitOfWorkResult
        : IUnitOfWorkResult
    {
        public UnitOfWorkResult(UnitOfWorkException exception)
        {
            this.Error = exception;
        }

        public UnitOfWorkException Error { get; }

        public bool IsSuccessfull()
        {
            return this.Error == null;
        }
    }
}
