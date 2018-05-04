using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWorks.Abstractions;

namespace UnitOfWorks.EntityFrameworkCore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkCoreUnitOfWork<TDbContext>(
            this IServiceCollection serviceCollection) 
            where TDbContext : DbContext
        {
            // Add the unit of work to the service collection. With scoped support.
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();

            return serviceCollection;
        }
    }
}
