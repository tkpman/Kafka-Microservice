using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UnitOfWorks.Abstractions
{
    public interface ISpecification<T>
        where T : class
    {
        /// <summary>
        /// Includes of sub children of base entity to get.
        /// </summary>
        IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// Includes of sub children of base entity to get.
        /// </summary>
        IReadOnlyCollection<string> IncludeStrings { get; }

        /// <summary>
        /// Adds the selected object in the expression to the
        /// list of entities to get from repository.
        /// </summary>
        /// <param name="expression">Expression to include entity.</param>
        void Include(Expression<Func<T, object>> expression);

        /// <summary>
        /// Adds the selected object with the help of a
        /// string, to find the object.
        /// </summary>
        /// <param name="include">String of object connection.</param>
        void Include(string include);
    }
}
