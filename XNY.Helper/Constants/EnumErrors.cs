using System.ComponentModel;

namespace XNY.Helper.Constants {
    /// <summary>
    /// 错误信息枚举
    /// </summary>
    public enum EnumErrors {
        #region 用户模块
        /// <summary>
        /// 用户或密码错误
        /// </summary>
        [Description("用户或密码错误")]
        User_NotValid = 100,
        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("用户不存在")]
        User_NotExist = 101,
        #endregion

        #region 授权
        /// <summary>
        /// 不是有效的应用
        /// </summary>
        [Description("不是有效的应用")]
        OAuth_NotValidApp = 200,

        /// <summary>
        /// 过期或已失效
        /// </summary>
        [Description("Token过期或已失效")]
        OAuth_NotValidToken = 201,

        /// <summary>
        /// 错误的授权方式
        /// </summary>
        [Description("错误的授权方式")]
        OAuth_GrantTypeNotExist = 202,

        /// <summary>
        /// Access Token已过期
        /// </summary>
        [Description("Access Token已过期")]
        OAuth_AccessTokenExpired = 203,

        /// <summary>
        /// 未授权的Ip地址
        /// </summary>
        [Description("未授权的Ip地址")]
        OAuth_NotValidIp = 204,

        /// <summary>
        /// 未授权的用户
        /// </summary>
        [Description("未授权的用户或密码错误")]
        OAuth_NotValidAccount =205,
        #endregion

        #region 应用
        /// <summary>
        /// 不是有效的应用
        /// </summary>
        [Description("不是有效的应用")]
        App_NotValidApp = 300,
        #endregion

        #region 应用
        /// <summary>
        /// 不是有效的任务
        /// </summary>
        [Description("不是有效的任务")]
        Task_NotValid = 400,
        #endregion

        #region 通用
        /// <summary>
        /// 常规错误
        /// </summary>
        [Description("常规错误")]
        NormalError = 900,
        /// <summary>
        /// 值为必填项
        /// </summary>
        [Description("值为必填项")]
        Required = 901,
        /// <summary>
        /// Email格式错误
        /// </summary>
        [Description("Email格式错误")]
        NotValidEmail = 902,
        /// <summary>
        /// 参数不能为空
        /// </summary>
        [Description("参数不能为空")]
        ParameterNotNull = 903
        #endregion
    }
}
