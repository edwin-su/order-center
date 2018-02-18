using Newtonsoft.Json;
using OrderCenterClient.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace OrderCenterClient.Filters
{
    public class CustomExceptionFilter : HandleErrorAttribute
    {
        public override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var message = JsonConvert.DeserializeObject<ExceptionModel>(filterContext.Exception.Message);

                message.Message = "错误";
                var jsonResult = new JsonResult()
                {
                    Data = message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.Result = jsonResult;
                return;
            }

            ViewResult veiwResult = new ViewResult
            {
                ViewName = "Page_400", //错误页
                MasterName = null,     //指定母版页
                ViewData = null,       //指定模型
                TempData = filterContext.Controller.TempData
            };

            filterContext.Result = veiwResult;
        }
    }
}