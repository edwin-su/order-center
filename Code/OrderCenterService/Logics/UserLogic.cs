using OrderCenterInterface.Models;
using OrderCenterService.Interfaces;
using OrderCenterService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterService.Logics
{
    public class UserLogic : IUserLogic
    {
        public List<OrderUser> GetUsers(string name)
        {
            return DBHelper.ExecuteDataReader<List<OrderUser>>("select order_operator_id, order_operator_name, order_operator_password from order_operator where order_operator_name = @name", new Parameter[] {
                    new Parameter(){
                         Key = "@name",
                          Value = name
                    }
            }, (reader) =>
            {
                var users = new List<OrderUser>();
                while (reader.Read())
                {
                    var user = new OrderUser()
                    {
                        ID = reader.GetInt64("order_operator_id"),
                        Name = reader.GetString("order_operator_name"),
                        Password = reader.GetString("order_operator_password")
                    };

                    users.Add(user);
                }

                return users;
            });
        }
    }
}