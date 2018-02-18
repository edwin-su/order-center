using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class OrderDetails
    {
        public string ReceiptNumber { get; set; }

        public string OrderAffectDay { get; set; }

        public string MealDetails { get; set; }

        public string AreaCode { get; set; }

        public string BedNumber { get; set; }

        public string PaymentMethod { get; set; }
    }
}