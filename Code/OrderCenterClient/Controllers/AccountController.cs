using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login() => View();

        [HttpGet]
        public ActionResult GetOperatorList()
        {
            var a = new Operator()
            {
                OperatorId=11,
                UserName = "张三",
                OperatorName = "zhangsan",
                AllowToViewOrder = true,
                IsActive = true,
            };
            var b = new Operator()
            {
                OperatorId = 22,
                UserName = "李四",
                OperatorName = "lisi",
                IsActive = false,
            };

            Operator[] array = { a, b };

            return Json(new { OperatorList = array }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateOperator(Operator operatorInfo)
        {

            return Json(new { UpdateSuccessfully = true });
        }

        [HttpPost]
        public ActionResult DeleteOperator(Operator operatorInfo)
        {
            operatorInfo.IsDelete = false;
            return Json(new { DeleteSuccessfully = true });
        }

        [HttpPost]
        public ActionResult ChangeAccountActiveStatus(Operator operatorInfo)
        {
            operatorInfo.IsActive = !operatorInfo.IsActive;
            return Json(new { ChangeStatusSuccessfully = true, OperatorStatus = operatorInfo.IsActive });
        }

        [HttpPost]
        public ActionResult CreateNewDefaultOperator(Operator operatorInfo)
        {
            return Json(new { CreateNewSuccessfully = true });
        }
    }
}