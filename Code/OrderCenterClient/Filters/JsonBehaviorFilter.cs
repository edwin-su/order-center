using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Filters
{
    public class JsonBehaviorFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var result = filterContext.Result as JsonResult;

            if (result != null)
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
        }
    }
}