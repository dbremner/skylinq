using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

namespace SkyLinq.Linq
{
    internal class SkyLinqQueryable<TElement> : IOrderedQueryable<TElement>
    {
        private IQueryProvider _provider;
        private Expression _expression;

        internal SkyLinqQueryable(IEnumerable<TElement> elements)
        {
            if (elements == null)
                throw new ArgumentNullException("provider");

            IQueryable<TElement> queryable = elements.AsQueryable<TElement>();
            _provider = new SkyLinqQueryProvider<TElement>(queryable);
            _expression = Expression.Constant(this);
        }

        internal SkyLinqQueryable(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<TElement>).GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
                throw new ArgumentOutOfRangeException("expression");

            _expression = expression;
            _provider = provider;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return ((IEnumerable<TElement>)_provider.Execute<TElement>(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TElement); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }
    }
}
