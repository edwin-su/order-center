using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using OrderCenterClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Logics
{
    public class PaymentMethodLogic : IPaymentMethodLogic
    {
        #region business logic

        public List<PaymentMethod> GetPaymentMethods()
        {
            return _GetPaymentMethods();
        }

        public bool TogglePaymentMethodStatus(int paymentMethodTypeId, bool isActive)
        {
            if (paymentMethodTypeId < 1)
            {
                throw new Exception("支付方式不存在");
            }
            return _TogglePaymentMethodStatus(paymentMethodTypeId, !isActive);
        }

        #endregion

        #region db logic

        private bool _TogglePaymentMethodStatus(int paymentMethodTypeId, bool isActive)
        {
            var sql = "UPDATE payment_method SET is_active = @is_active, last_updated = now() WHERE payment_method_type_id = @payment_method_type_id";
            var parameters = new Parameter[]
            {
                new Parameter("@is_active", isActive),
                new Parameter("@payment_method_type_id",paymentMethodTypeId),
            };
            return DBHelper.ExecuteNonQuery(sql, parameters);
        }

        private List<PaymentMethod> _GetPaymentMethods()
        {
            return DBHelper.ExecuteDataReader<List<PaymentMethod>>("select * from payment_method", null, (reader) =>
            {
                var paymentMethods = new List<PaymentMethod>();
                while (reader.Read())
                {
                    var paymentMethod = new PaymentMethod()
                    {
                        PaymentMethodTypeId = reader.GetInt32("payment_method_type_id"),
                        PayemtnMethodName = reader.GetString("payment_method_name"),
                        IsActive = reader.GetBoolean("is_active"),
                    };

                    paymentMethods.Add(paymentMethod);
                }

                return paymentMethods;
            });
        }

        #endregion
    }
}