using DapperExtensions.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    [Serializable]
    public class OrderBy<T> : OrderBy, IOrderBy<T> where T : class
    {

        public void AddOrderBy(Expression<Func<T, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderBy<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderBy<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderBy<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderBy<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderBy<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            base.AddOrderBy(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy));
        }

        public void AddOrderByDescending(Expression<Func<T, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public void AddOrderByDescending<T2>(Expression<Func<T, T2, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public void AddOrderByDescending<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public void AddOrderByDescending<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public void AddOrderByDescending<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }

        public void AddOrderByDescending<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaOrderBy)
        {
            base.AddOrderByDescending(ExpressionToClip<T>.ToOrderByClip(lambdaOrderBy, OrderByType.DESC));
        }
    }


    [Serializable]
    public class OrderBy : IOrderBy
    {
        private OrderByClip gb = OrderByClip.None;


        public void AddOrderBy(OrderByClip orderby)
        {
            gb = gb && orderby;
        }

        public void AddOrderByDescending(OrderByClip orderby)
        {
            gb = gb && orderby;
        }

        public OrderByClip ToOrderByClip()
        {
            return gb;
        }
    }
}
