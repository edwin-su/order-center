using OrderCenterInterface.Controllers;
using OrderCenterInterface.Models;
using OrderCenterService.Consts;
using OrderCenterService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OrderCenterService.Controllers
{
    public class UserController : AbstractUserController
    {
        private readonly IUserLogic _userLogic;
        private readonly ICache _cache;

        public UserController(IUserLogic userLogic, ICache cache)
        {
            _userLogic = userLogic;
            _cache = cache;
        }

        [Route("User/Login")]
        public override long Login(OrderCredential credential)
        {
            var users = _userLogic.GetUsers(credential.Name);
            var user = users.Where(u => u.Password == credential.Password).ToArray();

            if (user == null || user.Length == 0)
            {
                throw new Exception("用户名或密码不正确！");
            }

            //_cache.Add<long>(SessionConst.USER_ID, user[0].ID);


            return user[0].ID;
        }
    }
}