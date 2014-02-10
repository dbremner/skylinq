using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SkyLinq.Linq
{
    internal class SkyLinqQueryProvider<TElement> : IQueryProvider
    {
        private IQueryable<TElement> _baseQueryable;

        internal SkyLinqQueryProvider(IQueryable<TElement> baseQueryable)
        {
            _baseQueryable = baseQueryable;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new SkyLinqQueryable<TElement>(this, expression);
            //return _baseProvider.CreateQuery<TElement>(expression); //If I do this, I will lose the interception
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            //Need to translate the expression here or I will get argument error
            return _baseQueryable.Provider.Execute<TResult>(expression);
        }

        public object Execute(Expression expression)
        {
            return _baseQueryable.Provider.Execute(expression);
        }
    }
}
