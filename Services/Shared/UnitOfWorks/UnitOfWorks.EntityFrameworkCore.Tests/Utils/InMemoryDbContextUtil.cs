using System;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWorks.EntityFrameworkCore.Tests.Utils
{
    public static class InMemoryDbContextUtil
    {
        /// <summary>
        /// Creates an in memory database option with the
        /// given database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns>InMemory database options.</returns>
        public static DbContextOptions<InMemoryDbContext> 
            CreateInMemoryDatabaseOption(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            return new DbContextOptionsBuilder<InMemoryDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }
    }
}
