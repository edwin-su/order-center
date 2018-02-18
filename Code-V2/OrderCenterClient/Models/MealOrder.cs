using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class MealOrder
    {
        //public long Id { get; set; }

        public string BedNumber { get; set; }

        public int PatientAreaId { get; set; }

        public long OrderOperatorId { get; set; }

        public int DeviceTypeId { get; set; }

        //public DateTime DateCreated { get; set; }

        public int PaymentMethodId { get; set; }

        public string OrderAffectDay { get; set; }

        public string ReceiptNumber { get; set; }

        public List<Meal> Breakfasts { get; set; }

        public Meal Lunch { get; set; }

        public Meal Dinner { get; set; }
    }
}