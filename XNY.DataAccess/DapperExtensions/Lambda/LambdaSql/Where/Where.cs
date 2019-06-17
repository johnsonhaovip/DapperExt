using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    [Serializable]
    public class Where<T> : Where, IWhere<T> where T : class 
    {  
        /// <summary> 
        /// AND
        /// </summary>
        public void And(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        /// <summary>
        /// AND
        /// </summary>
        public void And<T2>(Expression<Func<T, T2, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        /// <summary>
        /// Or
        /// </summary>
        public void Or(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2>(Expression<Func<T, T2, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
    }


    [Serializable]
    public class Where : IWhere
    {

        /// <summary>
        /// 条件字符串
        /// </summary>
        private StringBuilder expressionStringBuilder = new StringBuilder();

        /// <summary>
        /// 条件参数
        /// </summary>
        private List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Where() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="where"></param>
        public Where(WhereClip where)
        {
            expressionStringBuilder.Append(where.ToString());

            parameters.AddRange(where.Parameters);

        }



        /// <summary>
        /// AND
        /// </summary>
        /// <param name="where"></param>
        public void And(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                return;


            if (expressionStringBuilder.Length > 0)
            {
                expressionStringBuilder.Append(" AND ");
                expressionStringBuilder.Append(where.ToString());
                expressionStringBuilder.Append(")");
                expressionStringBuilder.Insert(0, "(");
            }
            else
            {
                expressionStringBuilder.Append(where.ToString());
            }

            parameters.AddRange(where.Parameters);
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="where"></param>
        public void Or(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                return;


            if (expressionStringBuilder.Length > 0)
            {
                expressionStringBuilder.Append(" OR ");
                expressionStringBuilder.Append(where.ToString());
                expressionStringBuilder.Append(")");
                expressionStringBuilder.Insert(0, "(");
            }
            else
            {
                expressionStringBuilder.Append(where.ToString());
            }


            parameters.AddRange(where.Parameters);
        }


        /// <summary>
        /// 转换成WhereClip
        /// </summary>
        /// <returns></returns>
        public WhereClip ToWhereClip()
        {
            return new WhereClip(expressionStringBuilder.ToString(), parameters.ToArray());
        }
    }
}
