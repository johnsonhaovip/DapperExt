
using XNY.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace XNY.Freamework.Demo
{
    /// <summary>
    /// 为提供Demo  超级简化版
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 自己实现可配置
        /// </summary>
        public static bool EnabledDbSession { get { return false; } }

        public static IContainer IC { get; set; }

        public static IDBSession GetPerHttpRequestDBSession(string connkey = "DefaultConnection")
        {
            IDBSession dbSession;
            if (System.Web.HttpContext.Current.Items["IDBSession_" + connkey] != null)
            {
                dbSession = System.Web.HttpContext.Current.Items["IDBSession_" + connkey] as IDBSession;
            }
            else
            {
                if (connkey == "DefaultConnection")
                {
                    dbSession = Helper.IC.Resolve<IDBSession>();
                }
                else
                {
                    dbSession = Helper.IC.ResolveNamed<IDBSession>(connkey);
                }
                System.Web.HttpContext.Current.Items["IDBSession_" + connkey] = dbSession;
                System.Web.HttpContext.Current.Items["IDBSession_Keys"] += connkey + ",";
            }
            return dbSession;
        }


        public static void DisposePerHttpRequestDBSession()
        {
            string IDBSession_Keys = (System.Web.HttpContext.Current.Items["IDBSession_Keys"] ?? "").ToString();
            if (IDBSession_Keys.Length > 0)
            {
                foreach (string connkey in IDBSession_Keys.Split(','))
                {
                    if (!string.IsNullOrEmpty(connkey))
                    {
                        IDBSession dbSession = System.Web.HttpContext.Current.Items["IDBSession_" + connkey] as IDBSession;
                        if (dbSession != null)
                        {
                            dbSession.Dispose();
                        }
                    }
                }
            }
        }
    }
}
