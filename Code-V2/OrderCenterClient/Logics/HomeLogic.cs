using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using OrderCenterClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Logics
{
    public class HomeLogic : IHomeLogic
    {
        private readonly ICache _cache;

        public HomeLogic(ICache cache)
        {
            _cache = cache;
        }

        #region Business logic

        public long Login(string userName, string password)
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

            var users = _GetUsersByName(userName);
            var user = users.Where(u => u.Password == password).ToArray();

            if (user == null || user.Length == 0)
            {
                throw new Exception("用户名或密码不正确！");
            }

            var userId = user[0].OperatorId;

            _cache.Add<long>(SessionConstant.USER_ID, userId);
            _cache.Add<int>(SessionConstant.USER_PERMISSION_ID, user[0].OperatorPermissionId);

            return userId;
        }

        public List<Operator> GetOperatorList()
        {
            return _GetActiveOperatorList();
        }

        public bool CreateNewDefaultOperator(Operator operatorInfo, long operatorId)
        {
            if (operatorInfo == null || string.IsNullOrEmpty(operatorInfo.OperatorName) || string.IsNullOrEmpty(operatorInfo.Password))
            {
                throw new Exception("创建用户所需信息不足，请重试");
            }
            if (operatorId <1)
            {
                throw new Exception("未知操作员");
            }

            var operatorNames = _GetUsersByName(operatorInfo.OperatorName);
            var displayNames = _GetUsersByName(operatorInfo.OperatorName);
            if ((operatorNames!= null && operatorNames.Count > 0) || (displayNames != null && displayNames.Count > 0))
            {
                throw new Exception("该用户已存在，请更换用户名");
            }

            var permissionId = _CreateDefaultOperatorPermission(operatorId);
            return _CreateNewDefaultOperator(operatorInfo, permissionId);
        }

        public bool ChangeOperatorActiveStatus(Operator operatorInfo, long operatorId)
        {
            if (operatorInfo == null)
            {
                throw new Exception("账户信息有误，更新失败");
            }
            if (operatorId < 1)
            {
                throw new Exception("未知操作员");
            }
            return _ChangeOperatorActiveStatus(operatorInfo);
        }

        public bool UpdateOperator(Operator operatorInfo, long operatorId)
        {
            if (operatorInfo == null || string.IsNullOrEmpty(operatorInfo.OperatorName))
            {
                throw new Exception("更新用户所需信息不足，请重试");
            }
            if (operatorId < 1)
            {
                throw new Exception("未知操作员");
            }
            return _UpdateOperatorInfo(operatorInfo) && _UpdateOperatorPermission(operatorInfo, operatorId);
        }

        public bool DeleteOperator(Operator operatorInfo)
        {
            if (operatorInfo == null)
            {
                throw new Exception("删除用户所需信息不足，请重试");
            }

            return _DeleteOperator(operatorInfo);
        }

        public OperatorPermission GetOperatorPermission()
        {
            int permissionId = _cache.Get<int>(SessionConstant.USER_PERMISSION_ID);
            if (permissionId < 1)
            {
                throw new Exception("权限获取失败");
            }

            OperatorPermission permission = _GetOperatorPermission(permissionId);
            if (permission.IsSuperAdmin)
            {
                permission.AllowToViewOrder = true;
                permission.AllowToChangeOrder = true;
                permission.AllowToModifyMeal = true;
                permission.AllowToViewSales = true;
                permission.AllowToModifyAccount = true;
                permission.AllowToModifyPaymentMethod = true;
            }

            return permission;
        }

        #endregion


        #region DB logic

        private bool _DeleteOperator(Operator operatorInfo)
        {
            var sql = "UPDATE order_operator SET is_deleted = 1 WHERE order_operator_id = @order_operator_id";
            var parameters = new Parameter[]
            {
                new Parameter("@order_operator_id", operatorInfo.OperatorId),
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private bool _UpdateOperatorInfo(Operator operatorInfo)
        {
            var sql = "UPDATE order_operator SET order_operator_name = @order_operator_name";
            if (!string.IsNullOrEmpty(operatorInfo.Password))
            {
                sql += ", order_operator_password = @order_operator_password";
            }
            sql += ", last_updated = now() WHERE order_operator_id = @order_operator_id";
            var parameters = new Parameter[]
            {
                new Parameter("@order_operator_name", operatorInfo.OperatorName),
                new Parameter("@order_operator_password", operatorInfo.Password),
                new Parameter("@order_operator_id", operatorInfo.OperatorId),
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private bool _UpdateOperatorPermission(Operator operatorInfo, long operatorId)
        {
            var sql = "UPDATE operator_permission SET allow_to_view_order = @allow_to_view_order, allow_to_change_order = @allow_to_change_order" +
                ", allow_to_modify_meal = @allow_to_modify_meal, allow_to_view_sales = @allow_to_view_sales, allow_to_modify_account = @allow_to_modify_account, allow_to_modify_payment_method = @allow_to_modify_payment_method" +
                ", order_operator_id = @order_operator_id WHERE operator_permission_id = @operator_permission_id";
            var parameters = new Parameter[]
            {
                new Parameter("@allow_to_view_order", operatorInfo.AllowToViewOrder),
                new Parameter("@allow_to_change_order", operatorInfo.AllowToChangeOrder),
                new Parameter("@allow_to_modify_meal", operatorInfo.AllowToModifyMeal),
                new Parameter("@allow_to_view_sales", operatorInfo.AllowToViewSales),
                new Parameter("@allow_to_modify_account", operatorInfo.AllowToModifyAccount),
                new Parameter("@order_operator_id", operatorId),
                new Parameter("@operator_permission_id", operatorInfo.OperatorPermissionId),
                new Parameter("@allow_to_modify_payment_method", operatorInfo.AllowToModifyPaymentMethod),
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private List<Operator> _GetActiveOperatorList()
        {
            var sql = "select * from order_operator inner join operator_permission on order_operator.operator_permission_id = operator_permission.operator_permission_id WHERE order_operator.is_deleted = 0 and operator_permission.is_super_admin = 0";
            return DBHelper.ExecuteDataReader<List<Operator>>(sql, null, (reader) =>
            {
                var operators = new List<Operator>();
                while (reader.Read())
                {
                    var user = new Operator()
                    {
                        OperatorId = reader.GetInt32("order_operator_id"),
                        DisplayName = reader.GetString("display_name"),
                        OperatorName = reader.GetString("order_operator_name"),
                        Password = reader.GetString("order_operator_password"),
                        IsActive = reader.GetBoolean("is_active"),
                        AllowToViewOrder = reader.GetBoolean("allow_to_view_order"),
                        AllowToChangeOrder = reader.GetBoolean("allow_to_change_order"),
                        AllowToModifyMeal = reader.GetBoolean("allow_to_modify_meal"),
                        AllowToViewSales = reader.GetBoolean("allow_to_view_sales"),
                        AllowToModifyAccount = reader.GetBoolean("allow_to_modify_account"),
                        AllowToModifyPaymentMethod = reader.GetBoolean("allow_to_modify_payment_method"),
                        OperatorPermissionId = reader.GetInt32("operator_permission_id")
                    };

                    operators.Add(user);
                }

                return operators;
            });
        }

        private bool _ChangeOperatorActiveStatus(Operator operatorInfo)
        {
            var sql = "UPDATE order_operator SET is_active = @is_active WHERE order_operator_id = @order_operator_id";
            var parameters = new Parameter[]
            {
                new Parameter("@is_active", !operatorInfo.IsActive),
                new Parameter("@order_operator_id", operatorInfo.OperatorId),
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private List<Operator> _GetUsersByName(string name)
        {
            return DBHelper.ExecuteDataReader<List<Operator>>("select order_operator_id, order_operator_name, order_operator_password, operator_permission_id from order_operator where order_operator_name = @name", new Parameter[] {
                    new Parameter(){
                         Key = "@name",
                          Value = name
                    }
            }, (reader) =>
            {
                var users = new List<Operator>();
                while (reader.Read())
                {
                    var user = new Operator()
                    {
                        OperatorId = reader.GetInt32("order_operator_id"),
                        OperatorName = reader.GetString("order_operator_name"),
                        Password = reader.GetString("order_operator_password"),
                        OperatorPermissionId = reader.GetInt32("operator_permission_id"),
                    };

                    users.Add(user);
                }

                return users;
            });
        }

        private OperatorPermission _GetOperatorPermission(int permissionId)
        {
            var sql = "select * from operator_permission where operator_permission_id = @operator_permission_id;";
            return DBHelper.ExecuteDataReader<OperatorPermission>(sql, new Parameter[] {
                    new Parameter(){
                         Key = "@operator_permission_id",
                          Value = permissionId
                    }
            }, (reader) =>
            {
                OperatorPermission operatorPermission = null;
                if (reader.Read())
                {
                    if (operatorPermission == null)
                    {
                        operatorPermission = new OperatorPermission();
                    }
                    operatorPermission.AllowToViewOrder = reader.GetBoolean("allow_to_view_order");
                    operatorPermission.AllowToChangeOrder = reader.GetBoolean("allow_to_change_order");
                    operatorPermission.AllowToModifyMeal = reader.GetBoolean("allow_to_modify_meal");
                    operatorPermission.AllowToViewSales = reader.GetBoolean("allow_to_view_sales");
                    operatorPermission.AllowToModifyAccount = reader.GetBoolean("allow_to_modify_account");
                    operatorPermission.AllowToModifyPaymentMethod = reader.GetBoolean("allow_to_modify_payment_method");
                    operatorPermission.IsSuperAdmin = reader.GetBoolean("is_super_admin");
                }

                return operatorPermission;
            });
        }

        private bool _CreateNewDefaultOperator(Operator operatorInfo, int permissionId)
        {
            var sql = "insert into order_operator values(null, @operator_name, @operator_name,@password,1,0,@operator_permission_id,null,now(), now());";
            var parameters = new Parameter[]
            {
                new Parameter("@operator_name", operatorInfo.OperatorName),
                new Parameter("@password", operatorInfo.Password),
                new Parameter("@operator_permission_id", permissionId)
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private int _CreateDefaultOperatorPermission(long orderOperatorId)
        {
            var sql = "insert into operator_permission values(null, 0, 0, 0, 0, 0, 0, 0, @order_operator_id);SELECT LAST_INSERT_ID();";
            return DBHelper.ExecuteDataReader<int>(sql, new Parameter[] {
                    new Parameter(){
                         Key = "@order_operator_id",
                          Value = orderOperatorId
                    }
            }, (reader) =>
            {
                int permissionId = -1;
                if (reader.Read())
                {
                    permissionId = reader.GetInt32("LAST_INSERT_ID()");
                }

                return permissionId;
            });
        }

        #endregion
    }
}