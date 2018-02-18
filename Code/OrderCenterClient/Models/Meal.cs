using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class Meal
    {
        public string MealName { get; set; }

        public double Price { get; set; }

        public string MealDescription { get; set; }
    }
}