using OrderCenterClient.Interfaces;
using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderLogic _orderLogic;

        public OrderController(IOrderLogic orderLogic)
        {
            _orderLogic = orderLogic;
        }
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetOrders(string affectDate, int mealTypeId = 0, int mealId = 0, int areaId = 0, string bedNumber = null)
        {
            var date = DateTime.Now;
            if (!string.IsNullOrEmpty(affectDate))
            {
                DateTime.TryParse(affectDate, out date);
            }
            var result = _orderLogic.GetOrderList(date, mealTypeId, mealId, areaId, bedNumber);
            return Json(result);
        }

        public ActionResult GenerateMealOrder(MealOrder mealOrder)
        {
            var receiptNumber = _orderLogic.GenerateMealOrder(mealOrder);
            return Json(new { ReceiptNumber = receiptNumber });
        }

        public ActionResult GetFilterDays()
        {
            var today = DateTime.Now;
            List<string> dates = new List<string>();
            dates.Add(today.ToString("yyyy-MM-dd"));
            dates.Add(today.AddDays(-1).ToString("yyyy-MM-dd"));
            dates.Add(today.AddDays(1).ToString("yyyy-MM-dd"));

            return Json(new { Dates = dates });
        }

        public ActionResult GetSalesReport()
        {
            List<SalesData> salesData = _orderLogic.GetSalesData();
            return Json(new { SalesData = salesData });
        }
    }
}