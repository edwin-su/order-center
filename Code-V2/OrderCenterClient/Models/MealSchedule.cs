using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class MealSchedule
    {
        public long Id { get; set; }

        public long MealId { get; set; }

        public int MealDay { get; set; }

        public int MealType { get; set; }

        public Meal Meal { get; set; }
    }
}