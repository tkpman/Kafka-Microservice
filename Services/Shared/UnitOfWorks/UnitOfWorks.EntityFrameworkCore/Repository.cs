using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnitOfWorks.Abstractions;

namespace UnitOfWorks.EntityFrameworkCore
{
    public class Repository<TEntity>
        : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;

        private readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            this._dbContext = dbContext;
            this._dbSet = this._dbContext.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this._dbSet.Add(entity).Entity;
        }

        public async Task<IReadOnlyCollection<TEntity>> All(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this._dbSet.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<TEntity>> All(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            var query = ConstructQueryFromSpecification(specification, this._dbSet);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> FirstOrDefault(
            Expression<Func<TEntity, bool>> expression, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await this._dbSet.FirstOrDefaultAsync(expression, cancellationToken);
        }

        public async Task<TEntity> FirstOrDefault(
            Expression<Func<TEntity, bool>> expression, 
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            var query = ConstructQueryFromSpecification(specification, this._dbSet);

            return await query.FirstOrDefaultAsync(expression, cancellationToken);
        }

        public TEntity Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this._dbSet.Remove(entity).Entity;
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this._dbSet.Update(entity).Entity;
        }

        public async Task<IReadOnlyCollection<TEntity>> Where(
            Expression<Func<TEntity, bool>> expression, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await this._dbSet.Where(expression).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<TEntity>> Where(
            Expression<Func<TEntity, bool>> expression, 
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            var query = ConstructQueryFromSpecification(specification, this._dbSet);

            return await query.Where(expression).ToListAsync(cancellationToken);
        }

        protected IQueryable<TEntity> ConstructQueryFromSpecification(
            ISpecification<TEntity> specification, DbSet<TEntity> set)
        {
            // Fetch a Queryable that includes all expression-based includes.
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(set.AsQueryable(),
                    (current, include) => current.Include(include));

            // Modify the IQueryable to include any string-based include 
            // statements.
            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return secondaryResult;
        }
    }
}
