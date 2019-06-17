using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Common.Attibues;

namespace WebApi.Controllers
{
    /// <summary>
    /// API基类
    /// </summary>
    [CompressionAttribute]
    public class ApiBaseController : Controller
    {
    }
}