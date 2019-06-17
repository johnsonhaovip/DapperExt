using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    public interface IOrderBy<T> : IOrderBy where T : class
    {

        void AddOrderBy(Expression<Func<T, object>> lambdaOrderBy);
        void AddOrderBy<T2>(Expression<Func<T, T2, object>> lambdaOrderBy);
        void AddOrderBy<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy);
        void AddOrderBy<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy);
        void AddOrderBy<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy);
        void AddOrderBy<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy);

        void AddOrderByDescending(Expression<Func<T, object>> lambdaOrderBy);
        void AddOrderByDescending<T2>(Expression<Func<T, T2, object>> lambdaOrderBy);
        void AddOrderByDescending<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy);
        void AddOrderByDescending<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy);
        void AddOrderByDescending<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy);
        void AddOrderByDescending<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy); 
    }


    public interface IOrderBy  
    {

        void AddOrderBy(OrderByClip orderby); 

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="where"></param>
        void AddOrderByDescending(OrderByClip orderby);

         
        /// <summary>
        /// 转换成WhereClip
        /// </summary>
        /// <returns></returns>
        OrderByClip ToOrderByClip(); 
    }
}
