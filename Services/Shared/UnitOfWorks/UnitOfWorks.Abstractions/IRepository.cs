using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitOfWorks.Abstractions
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Adds the given entity to the repository.
        /// </summary>
        /// <param name="entity">Entity to add to repository.</param>
        /// <returns>Entity which got added.</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Gets all the entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities which matches the expression.</returns>
        Task<IReadOnlyCollection<TEntity>> All(
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all the entities in the repository.
        /// </summary>
        /// <param name="specification">Specification to use to get sub entities.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities which matches the expression.</returns>
        Task<IReadOnlyCollection<TEntity>> All(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the first entity which matches the given expression, if no entity
        /// is found return default value.
        /// </summary>
        /// <param name="expression">Expression to use for finding entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefault(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the first entity which matches the given expression, if no entity
        /// is found return default value.
        /// </summary>
        /// <param name="expression">Expression to use for finding entity.</param>
        /// <param name="specification">Specification to use to get sub entities.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefault(
            Expression<Func<TEntity, bool>> expression,
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes the given entity from the repository.
        /// </summary>
        /// <param name="entity">Entity to remove from repository.</param>
        /// <returns>Entity which got removed.</returns>
        TEntity Remove(TEntity entity);

        /// <summary>
        /// Updates the given entity in the repository.
        /// </summary>
        /// <param name="entity">Entity to update in repository.</param>
        /// <returns>Entity which got updated.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Gets all entities which matches the given expression in the
        /// repository.
        /// </summary>
        /// <param name="expression">Expression to use for finding entities.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities which match the given epxression.</returns>
        Task<IReadOnlyCollection<TEntity>> Where(
            Expression<Func<TEntity, bool>> expression,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all entities which matches the given expression in the
        /// repository.
        /// </summary>
        /// <param name="expression">Expression to use for finding entities.</param>
        /// <param name="specification">Specification to use to get sub entities.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities which match the given epxression.</returns>
        Task<IReadOnlyCollection<TEntity>> Where(
            Expression<Func<TEntity, bool>> expression,
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
