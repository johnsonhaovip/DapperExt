using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    [Serializable]
    public class Select<T> : Select, ISelect<T> where T : class
    {
        public void AddSelect(Expression<Func<T, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2>(Expression<Func<T, T2, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }


        public void AddSelect(Expression<Func<T, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2>(Expression<Func<T, T2, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3>(Expression<Func<T, T2, T3, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }
        public void AddSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> lambdaSelect)
        {
            base.AddSelect(ExpressionToClip<T>.ToSelect(lambdaSelect));
        }

    }



    [Serializable]
    public class Select : ISelect
    {
        private List<Field> _Fields = new List<Field>();

        public List<Field> Fields
        {
            get { return _Fields; }
            private set { _Fields = value; }
        }



        public void AddSelect(params Field[] fields)
        {
            if (null != fields && fields.Length > 0)
            {
                foreach (Field field in fields)
                {
                    Field f = this._Fields.Find(fi => fi.Name.Equals(field.Name) && fi.TableName.Equals(field.TableName));
                    if (Field.IsNullOrEmpty(f))
                        this._Fields.Add(field);
                }
            }
        }
    }
}
