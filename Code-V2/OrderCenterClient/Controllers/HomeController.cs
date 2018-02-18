using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeLogic _homeLogic;
        private readonly ICache _cache;

        public HomeController(IHomeLogic homeLogic, ICache cache)
        {
            _homeLogic = homeLogic;
            _cache = cache;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var userId = _cache.Get<long>(SessionConstant.USER_ID);
            if (userId == -1 && Request.Url.LocalPath != "/")
            {
                return RedirectToRoute("Login");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            _homeLogic.Login(userName, password);
            return Json(new { LoginSuccess = true});
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Abandon();
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetOperatorPermission()
        {
            return Json(new { OperatorPermission = _homeLogic.GetOperatorPermission() });
        }

        [HttpGet]
        public ActionResult GetOperatorList()
        {
            List<Operator> operators = _homeLogic.GetOperatorList();

            return Json(new { OperatorList = operators });
        }

        [HttpPost]
        public ActionResult UpdateOperator(Operator operatorInfo)
        {
            return Json(new { UpdateSuccessfully = _homeLogic.UpdateOperator(operatorInfo, _cache.Get<long>(SessionConstant.USER_ID)) });
        }

        [HttpPost]
        public ActionResult DeleteOperator(Operator operatorInfo)
        {
            return Json(new { DeleteSuccessfully = _homeLogic.DeleteOperator(operatorInfo) });
        }

        [HttpPost]
        public ActionResult ChangeOperatorActiveStatus(Operator operatorInfo)
        {
            return Json(new { ChangeStatusSuccessfully = _homeLogic.ChangeOperatorActiveStatus(operatorInfo, _cache.Get<long>(SessionConstant.USER_ID)), OperatorStatus = !operatorInfo.IsActive });
        }

        [HttpPost]
        public ActionResult CreateNewDefaultOperator(Operator operatorInfo)
        {
            return Json(new { CreateNewSuccessfully = _homeLogic.CreateNewDefaultOperator(operatorInfo, _cache.Get<long>(SessionConstant.USER_ID)) });
        }
    }
}