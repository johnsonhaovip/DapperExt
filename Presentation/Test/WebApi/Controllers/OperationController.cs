using Api.Service.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;
using XNY.Helper;
using XNY.Helper.Extensions;

namespace WebApi.Controllers
{
    /// <summary>
    /// 操作控制器
    /// </summary>
    [RoutePrefix("api/operation")]
    public class OperationController : ApiBaseController
    {
        #region Fields
        /// <summary>
        /// 项目连接字符
        /// </summary>
        public const string ModuleName = "Test";

        private readonly ApiTestService apiTestService;

        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public OperationController()
        {
            apiTestService = new ApiTestService(ModuleName);
        }

        /// <summary>
        /// 根据id获取所有数据
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        //[HttpPost, RequireAuthorization(ModuleName), Route("getalldata")]
        [HttpPost,Route("getalldata")]
        public ResultJson<List<Student>> GetAllData([FromBody]ParamStudent param)
        {
            var result = new ResultJson<List<Student>>()
            {
                code = 0,
                message = "获取失败"
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    result.message = ModelState.Values.First(w => w.Errors.Count > 0).Errors[0].ErrorMessage;
                    return result;
                }
                var datas = this.apiTestService.GetAllData(param.id);
                List<Student> data = null;
                if (datas != null)
                {
                    data = datas.Select(q => new Student
                    {
                        Id = q.Id,
                        Number = q.Number,
                        Sex = q.Sex,
                        Phone = q.Phone
                    }).ToList();
                }
                result.code = 1;
                result.message = "获取成功";
                result.data = data;
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return result;
            }
        }

    }
}
