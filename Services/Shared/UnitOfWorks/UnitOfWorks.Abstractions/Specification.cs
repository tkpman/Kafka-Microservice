using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UnitOfWorks.Abstractions
{
    public class Specification<T>
        : ISpecification<T> where T : class
    {
        private readonly List<Expression<Func<T, object>>> _includes;

        private readonly List<string> _includeStrings;

        public Specification()
        {
            this._includes = new List<Expression<Func<T, object>>>();
            this._includeStrings = new List<string>();
        }

        public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes;

        public IReadOnlyCollection<string> IncludeStrings => _includeStrings;

        public void Include(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            this._includes.Add(expression);
        }

        public void Include(string include)
        {
            if (string.IsNullOrWhiteSpace(include))
                throw new ArgumentNullException(nameof(include));

            this._includeStrings.Add(include);
        }
    }
}
