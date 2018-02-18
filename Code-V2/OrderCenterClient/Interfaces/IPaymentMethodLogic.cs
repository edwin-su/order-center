using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterClient.Interfaces
{
    public interface IPaymentMethodLogic
    {
        List<PaymentMethod> GetPaymentMethods();

        bool TogglePaymentMethodStatus(int paymentMethodTypeId, bool isActive);
    }
}
