using System.Threading;
using System.Threading.Tasks;

namespace UnitOfWorks.Abstractions
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Get the repository which match the entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        /// <summary>
        /// Saves all the changes in the unit of work.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>UnitOfWorkResult with the result of the save.</returns>
        Task<IUnitOfWorkResult> SaveChanges(
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
