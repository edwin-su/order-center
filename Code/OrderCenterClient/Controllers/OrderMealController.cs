using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class OrderMealController : Controller
    {
        public ActionResult GetMealToOrder(bool isToday = false)
        {
            List<Meal> meals = new List<Meal>();
            for (int i = 0; i < 6; i ++)
            {
                Meal m = new Meal();
                m.MealName = "红烧肉";
                m.MealDescription = "葱，姜，蒜";
                m.Price = 25.00;
                meals.Add(m);
            }

            List<Area> areas = new List<Area>();
            for (int i = 1; i < 10; i++)
            {
                Area area = new Area();
                area.AreaId = i;
                area.AreaName = "" + i + "A";
                Area area1 = new Area();
                area1.AreaId = i;
                area1.AreaName = "" + i + "B";
                areas.Add(area);
                areas.Add(area1);
            }

            return Json(new { Meals = meals, Areas = areas });
        }
    }
}