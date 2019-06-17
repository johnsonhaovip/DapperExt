using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    public interface IWhere<T> : IWhere where T : class
    {

        void And(Expression<Func<T, bool>> lambdaWhere); 
        void And<T2>(Expression<Func<T, T2, bool>> lambdaWhere);
        void And<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere);
        void And<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere);
        void And<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere);
        void And<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere);

        void Or(Expression<Func<T, bool>> lambdaWhere);
        void Or<T2>(Expression<Func<T, T2, bool>> lambdaWhere);
        void Or<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere);
        void Or<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere);
        void Or<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere);
        void Or<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere);
    }


    public interface IWhere 
    {  

        void And(WhereClip where);

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="where"></param>
        void Or(WhereClip where);


        /// <summary>
        /// 转换成WhereClip
        /// </summary>
        /// <returns></returns>
        WhereClip ToWhereClip();
    }
}
