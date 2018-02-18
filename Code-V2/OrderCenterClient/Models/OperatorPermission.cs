using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class OperatorPermission
    {
        public int OperatorPermissionId { get; set; } 

        public bool AllowToViewOrder { get; set; }

        public bool AllowToChangeOrder { get; set; }

        public bool AllowToModifyMeal { get; set; }

        public bool AllowToViewSales { get; set; }

        public bool AllowToModifyAccount { get; set; }

        public bool AllowToModifyPaymentMethod { get; set; }

        public bool IsSuperAdmin { get; set; }

        public int OrderOperatorId { get; set; }
    }
}