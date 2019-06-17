using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XNY.Helper.UI.Scripts;

namespace XNY.Helper.Extensions {
    public static class ControllerExtension {
        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="message">消息</param>
        public static void Tips(this Controller controller, string message) {
            controller.TempData["scripts"] += JavascriptExtension.Tip(message);
        }

        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="operation">业务操作反馈</param>
        public static void Tips<T>(this Controller controller, OperationResult<T> operation) {
            controller.TempData["scripts"] += JavascriptExtension.Tip(operation.Message);
        }

        /// <summary>
        /// 提示消息
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="operation">业务操作反馈</param>
        public static void Notice(this Controller controller, string title, string message) {
            controller.TempData["scripts"] += JavascriptExtension.Notice(title, message);
        }
    }
}
