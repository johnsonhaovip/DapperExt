﻿using DapperExtensions.Mapper;
using DapperExtensions.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperExtensions.Lambda
{
    [Serializable]
    public class OrderByClip
    {

        private Dictionary<string, OrderByType> orderByClip = new Dictionary<string, OrderByType>();

        /// <summary>
        /// null
        /// </summary>
        public readonly static OrderByClip None = new OrderByClip();

        private OrderByClip() { }

        public OrderByClip(string fieldName, OrderByType orderBy)
        {
            orderByClip.Add(fieldName, orderBy);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="field"></param>
        public OrderByClip(Field field)
            : this(field.TableFieldName, OrderByType.ASC)
        {

        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="field"></param>
        /// <param name="orderBy"></param>
        public OrderByClip(Field field, OrderByType orderBy)
            : this(field.TableFieldName, orderBy)
        {

        }
        /// <summary>
        /// 判断 OrderByClip  是否为null
        /// </summary>
        /// <param name="orderByClip"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(OrderByClip orderByClip)
        {
            if ((null == orderByClip) || string.IsNullOrEmpty(orderByClip.ToString()))
                return true;
            return false;
        }

        /// <summary>
        /// 两个OrderByClip相加
        /// </summary>
        /// <param name="leftOrderByClip"></param>
        /// <param name="rightOrderByClip"></param>
        /// <returns></returns>
        public static OrderByClip operator &(OrderByClip leftOrderByClip, OrderByClip rightOrderByClip)
        {
            if (IsNullOrEmpty(leftOrderByClip) && IsNullOrEmpty(rightOrderByClip))
                return None;

            if (IsNullOrEmpty(leftOrderByClip))
                return rightOrderByClip;
            if (IsNullOrEmpty(rightOrderByClip))
                return leftOrderByClip;


            OrderByClip orderby = new OrderByClip();
            foreach (KeyValuePair<string, OrderByType> kv in leftOrderByClip.orderByClip)
            {
                orderby.orderByClip.Add(kv.Key, kv.Value);
            }

            foreach (KeyValuePair<string, OrderByType> kv in rightOrderByClip.orderByClip)
            {
                if (!orderby.orderByClip.ContainsKey(kv.Key))
                    orderby.orderByClip.Add(kv.Key, kv.Value);
            }

            return orderby;
        }

        public static bool operator true(OrderByClip right)
        {
            return false;
        }

        public static bool operator false(OrderByClip right)
        {
            return false;
        }


        /// <summary>
        /// 去掉的表前缀
        /// </summary>
        /// <returns></returns>
        public OrderByClip RemovePrefixTableName()
        {
            OrderByClip tempOrderByClip = new OrderByClip();

            foreach (KeyValuePair<string, OrderByType> kv in this.orderByClip)
            {
                string keyName = kv.Key;
                if (kv.Key.IndexOf('.') > 0)
                    keyName = keyName.Substring(keyName.IndexOf('.') + 1);

                tempOrderByClip.orderByClip.Add(keyName, kv.Value);
            }

            return tempOrderByClip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder orderBy = new StringBuilder();
            foreach (KeyValuePair<string, OrderByType> kv in this.orderByClip)
            {
                orderBy.Append(",");
                orderBy.Append(kv.Key);
                orderBy.Append(" ");
                orderBy.Append(kv.Value.ToString());
            }
            if (orderBy.Length > 1)
                return orderBy.ToString().Substring(1);
            return orderBy.ToString();
        }

        /// <summary>
        /// 倒叙
        /// </summary>
        public OrderByClip ReverseOrderByClip
        {
            get
            {
                OrderByClip tempOrderByClip = new OrderByClip();

                foreach (KeyValuePair<string, OrderByType> kv in this.orderByClip)
                {
                    tempOrderByClip.orderByClip.Add(kv.Key, kv.Value == OrderByType.ASC ? OrderByType.DESC : OrderByType.ASC);
                }

                return tempOrderByClip;

            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="orderByClip"></param>
        /// <returns></returns>
        public bool Equals(OrderByClip orderByClip)
        {
            if (null == orderByClip)
                return false;
            return this.ToString().Equals(orderByClip.ToString());
        }


        /// <summary>
        /// OrderByString
        /// <example>
        /// order by id desc
        /// </example>
        /// </summary>
        public string OrderByString
        {
            get
            {
                if (this.orderByClip.Count == 0)
                    return string.Empty;

                return string.Concat(" ORDER BY ", this.ToString());
            }
        }

    }
}
