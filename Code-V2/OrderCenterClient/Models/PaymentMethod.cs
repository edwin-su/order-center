using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class PaymentMethod
    {
        public int PaymentMethodTypeId { get; set; }

        public string PayemtnMethodName { get; set; }

        public bool IsActive { get; set; }
    }
}