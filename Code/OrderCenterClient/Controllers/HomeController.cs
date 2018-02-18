using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Utilities;
using OrderCenterInterface;
using OrderCenterInterface.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            _homeLogic.AddUser(userName, 1);
            return Json(new { Success = "true" });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Abandon();
            return Json(true);
        }
    }
}