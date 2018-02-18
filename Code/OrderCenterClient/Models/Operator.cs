using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class Operator
    {
        public int OperatorId { get; set; }

        public string OperatorName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public bool AllowToViewOrder { get; set; }

        public bool AllowToChangeOrder { get; set; }

        public bool AllowToModifyMeal { get; set; }

        public bool AllowToViewSales { get; set; }

        public bool AllowToModifyAccount { get; set; }

        public bool AllowToModifyPaymentMethod { get; set; }
    }
}