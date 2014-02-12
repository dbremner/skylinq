using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace SkyLinq.Linq
{
    internal class SkyLinqQueryProvider : IQueryProvider
    {
        private IEnumerable _enumerable;

        internal SkyLinqQueryProvider(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            _enumerable = enumerable;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            Type type = TypeHelper.FindGenericType(typeof(IQueryable<>), expression.Type);
            if (type == null)
            {
                throw new ArgumentException("expression");
            }

            return new SkyLinqQueryable<TElement>(this, expression);
            //return _baseProvider.CreateQuery<TElement>(expression); //If I do this, I will lose the interception
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            Type type = TypeHelper.FindGenericType(typeof(IQueryable<>), expression.Type);
            if (type == null)
            {
                throw new ArgumentException("expression");
            }
            return Create(type.GetTypeInfo().GenericTypeArguments.First(), expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            //Need to translate the expression here or I will get argument error
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        private IQueryable Create(Type elementType, Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[] { elementType }), this, expression);
        }
    }
}
