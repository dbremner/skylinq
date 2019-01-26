using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

namespace SkyLinq.Composition
{
    internal abstract class SkyLinqQuery : IOrderedQueryable
    {
        internal abstract IEnumerable Enumerable { get; }

        public abstract Type ElementType { get; }

        public abstract Expression Expression { get; }

        public abstract IQueryProvider Provider { get; }

        public abstract IEnumerator GetEnumerator();
    }

    internal sealed class SkyLinqQuery<TElement> : SkyLinqQuery, IOrderedQueryable<TElement>
    {
        private readonly IQueryProvider _provider;
        private readonly Expression _expression;
        private readonly IEnumerable<TElement> _elements;

        internal SkyLinqQuery(IEnumerable<TElement> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            IQueryable<TElement> queryable = elements.AsQueryable<TElement>();
            _elements = elements;
            _provider = new SkyLinqQueryProvider(queryable);
            _expression = Expression.Constant(this);
        }

        internal SkyLinqQuery(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof(IQueryable<TElement>).GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            _expression = expression;
            _provider = provider;
        }

        public override Expression Expression
        {
            get { return _expression; }
        }

        internal override IEnumerable Enumerable
        {
            get { return _elements; }
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return ((IEnumerable<TElement>)_provider.Execute<IEnumerable<TElement>>(_expression)).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
        }

        public override Type ElementType
        {
            get { return typeof(TElement); }
        }

        public override IQueryProvider Provider
        {
            get { return _provider; }
        }
    }
}
