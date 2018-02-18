using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class Meal
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public int MealType { get; set; }

        public bool IsBreakfast { get; set; }

        public bool IsSelected { get; set; }
    }
}