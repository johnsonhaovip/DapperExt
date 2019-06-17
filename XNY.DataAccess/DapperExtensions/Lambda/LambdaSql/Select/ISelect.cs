using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{

    public interface ISelect<T> : ISelect where T : class
    {

        void AddSelect(Expression<Func<T, bool>> lambdaSelect);
        void AddSelect<T2>(Expression<Func<T, T2, bool>> lambdaSelect);
        void AddSelect<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaSelect);
        void AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaSelect);
        void AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaSelect);
        void AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaSelect);

        void AddSelect(Expression<Func<T, object>> lambdaSelect);
        void AddSelect<T2>(Expression<Func<T, T2, object>> lambdaSelect);
        void AddSelect<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaSelect);
        void AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaSelect);
        void AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaSelect);
        void AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaSelect);

    }

    public interface ISelect
    {
        List<Field> Fields { get; }

        void AddSelect(params Field[] fields);
    }

}
