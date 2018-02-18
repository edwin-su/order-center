using OrderCenterClient.Consts;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using OrderCenterClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OrderCenterClient.Logics
{
    public class OrderLogic : IOrderLogic
    {
        private readonly ICache _cache;

        public OrderLogic(ICache cache)
        {
            _cache = cache;
        }

        public List<OrderDetails> GetOrderList(DateTime affectDate, int mealTypeId, int mealId, int areaId, string bedNumber)
        {
            if (affectDate == DateTime.MinValue)
            {
                throw new Exception("所选日期不正确！");
            }

            if (mealTypeId < 0)
            {
                throw new Exception("餐品类型不正确");
            }

            if (mealId < 0)
            {
                throw new Exception("请选择正确的菜品");
            }

            if (areaId < 0)
            {
                throw new Exception("请选择正确的病区");
            }

            return _GetOrderList(affectDate, mealTypeId, mealId, areaId, bedNumber);
        }

        public string GenerateMealOrder(MealOrder mealOrder)
        {
            if (mealOrder == null || (mealOrder.Breakfasts == null && mealOrder.Lunch == null && mealOrder.Dinner == null) || string.IsNullOrEmpty(mealOrder.BedNumber) || mealOrder.PatientAreaId < 1)
            {
                throw new Exception("订餐信息不全，订单生成失败");
            }
            mealOrder.OrderAffectDay = DateTime.Now.AddDays(1).Date.ToString("yyy-MM-dd");
            mealOrder.OrderOperatorId = _cache.Get<long>(SessionConstant.USER_ID);
            mealOrder.ReceiptNumber = DateTime.Now.ToFileTime().ToString().Substring(12, 6);

            var orderId = _GenerateOrder(mealOrder);

            foreach (var item in mealOrder.Breakfasts)
            {
                _GenerateMealOrder(item, orderId);
            }
            if (mealOrder.Lunch != null)
            {
                _GenerateMealOrder(mealOrder.Lunch, orderId);
            }
            if (mealOrder.Dinner != null)
            {
                _GenerateMealOrder(mealOrder.Dinner, orderId);
            }

            return mealOrder.ReceiptNumber;
        }

        public List<SalesData> GetSalesData()
        {
            return _GetSalesData();
        }

        private List<SalesData> _GetSalesData()
        {
            var sql = "select count(*),order_affect_day from `order` group by order_affect_day order by order_affect_day asc;";
            return DBHelper.ExecuteDataReader<List<SalesData>>(sql, null, (reader) =>
            {
                var salesData = new List<SalesData>();
                while (reader.Read())
                {
                    salesData.Add(new SalesData()
                    {
                        SalesCount = reader.GetInt64("count(*)"),
                        OrderAffectDay = reader.GetDateTime("order_affect_day").ToString("yyyy-MM-dd"),
                    });
                }
                return salesData;
            });
        }

        private long _GenerateOrder(MealOrder mealOrder)
        {
            var sql = "INSERT INTO `order` VALUES(NULL,@bed_number,@patient_area_id,@order_operator_id,@device_type_id,now(),@payment_method_id,@order_affect_day,@receipt_number);SELECT LAST_INSERT_ID();";
            var parameters = new Parameter[]
            {
                new Parameter("@bed_number", mealOrder.BedNumber),
                new Parameter("@patient_area_id", mealOrder.PatientAreaId),
                new Parameter("@order_operator_id", mealOrder.OrderOperatorId),
                new Parameter("@device_type_id", mealOrder.DeviceTypeId),
                new Parameter("@payment_method_id", mealOrder.PaymentMethodId),
                new Parameter("@order_affect_day", mealOrder.OrderAffectDay),
                new Parameter("@receipt_number", mealOrder.ReceiptNumber)
            };

            return DBHelper.ExecuteDataReader<long>(sql, parameters, (reader) =>
            {
                long id = -1;
                if (reader.Read())
                {
                    id = reader.GetInt32("LAST_INSERT_ID()");
                }

                return id;
            });
        }

        private List<OrderDetails> _GetOrderList(DateTime affectDate, int mealTypeId, int mealId, int areaId, string bedNumber)
        {
            var parameters = new List<Parameter>() {
                new Parameter("@order_affect_day", affectDate)
            };
            var sql = new StringBuilder(@"select o.receipt_number,o.order_affect_day,GROUP_CONCAT(mt.meal_type_name,':', m.meal_name) as meal_details,
                        pa.area_code, o.bed_number, pm.payment_method_name from `order` o
                        inner join meal_order mo
                        on o.id = mo.order_id
                        inner join meal m
                        on mo.meal_id = m.meal_id
                        inner join patient_area pa
                        on o.patient_area_id = pa.id
                        inner join payment_method pm
                        on o.payment_method_id = pm.payment_method_type_id
                        inner join lu_meal_type mt
                        on mo.meal_type = mt.meal_type_id
                        where o.order_affect_day = @order_affect_day ");
            if (mealTypeId > 0)
            {
                sql.Append("and mo.meal_type = @meal_type ");
                parameters.Add(new Parameter("@meal_type", mealTypeId));
            }

            if (mealId > 0)
            {
                sql.Append("and mo.meal_id = @meal_id ");
                parameters.Add(new Parameter("@meal_id", mealId));
            }

            if (areaId > 0)
            {
                sql.Append("and o.patient_area_id = @area_id ");
                parameters.Add(new Parameter("@area_id", areaId));
            }

            if (!string.IsNullOrEmpty(bedNumber))
            {
                sql.Append("and o.bed_number = @bed_number ");
                parameters.Add(new Parameter("@bed_number", bedNumber));
            }

            sql.Append("group by o.receipt_number,o.order_affect_day,pa.area_code,o.bed_number,pm.payment_method_name");

            return DBHelper.ExecuteDataReader<List<OrderDetails>>(sql.ToString(), parameters.ToArray(), (reader) =>
            {
                var orderDetails = new List<OrderDetails>();
                while (reader.Read())
                {
                    orderDetails.Add(new OrderDetails()
                    {
                        AreaCode = reader.GetString("area_code"),
                        ReceiptNumber = reader.GetString("receipt_number"),
                        OrderAffectDay = reader.GetDateTime("order_affect_day").ToString("yyyy-MM-dd"),
                        MealDetails = reader.GetString("meal_details"),
                        BedNumber = reader.GetString("bed_number"),
                        PaymentMethod = reader.GetString("payment_method_name")
                    });
                }
                return orderDetails;
            });
        }

        private bool _GenerateMealOrder(Meal meal, long orderId)
        {
            var sql = "insert into `meal_order` values(null, @order_id, @meal_type,@meal_id)";
            var parameters = new Parameter[]
            {
                new Parameter("@order_id", orderId),
                new Parameter("@meal_type", meal.MealType),
                new Parameter("@meal_id", meal.Id),
            };

            return DBHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}