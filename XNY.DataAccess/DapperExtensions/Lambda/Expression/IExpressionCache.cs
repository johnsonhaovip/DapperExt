using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DapperExtensions.Lambda
{
    public interface IExpressionCache<T> where T : class
    {
        T Get(System.Linq.Expressions.Expression key, Func<System.Linq.Expressions.Expression, T> creator);
    }
}
