using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace challenge1.Common.Util.Common.Helpers
{
    public static class DatabaseHelper
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool descending = false)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var keySelector = Expression.Lambda(selector, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";

            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { source.ElementType, selector.Type },
                source.Expression,
                Expression.Quote(keySelector));

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
