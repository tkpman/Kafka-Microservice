using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnitOfWorks.Abstractions;

namespace UnitOfWorks.EntityFrameworkCore
{
    public class UnitOfWork<TDbContext>
        : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly IDictionary<Type, object> _repositories;

        public UnitOfWork(TDbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            this._dbContext = dbContext;

            this._repositories = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!this._repositories.ContainsKey(type))
                this._repositories.Add(type, new Repository<TEntity>(this._dbContext));

            return (IRepository<TEntity>)this._repositories[type];
        }

        public async Task<IUnitOfWorkResult> SaveChanges(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var result = await this._dbContext.SaveChangesAsync(cancellationToken);

                // Give the unit of work result no exception, so that it knows
                // nothing went wrong, in saving the changes.
                return new UnitOfWorkResult(null);
            }
            catch (DbUpdateException exception)
            {
                var unitOfWorkException = new UnitOfWorkException(
                    "UnitOfWork failed to save changes.", 
                    exception);

                return new UnitOfWorkResult(unitOfWorkException);
            }
        }
    }
}
