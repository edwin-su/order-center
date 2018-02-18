using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Utilities;
using OrderCenterInterface.Controllers;
using OrderCenterInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Logics
{
    public class HomeLogic : IHomeLogic
    {
        private readonly AbstractUserController _userController;
        private readonly ICache _cache;

        public HomeLogic(AbstractUserController userController, ICache cache)
        {
            _userController = userController;
            _cache = cache;
        }

        public bool Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new Exception("用户名不能为空");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("密码不能为空");
            }

            //if (MD5Helper.MD5Encrypt64(password).Equals(centerUser.password))
            var credential = new OrderCredential()
            {
                Name = userName,
                Password = password
            };

            var userId = _userController.Login(credential);

            _cache.Add<long>(SessionConstant.USER_ID, userId);

            return true;
        }

        public bool AddUser(string name, int age)
        {

            return true;
        }
    }
}