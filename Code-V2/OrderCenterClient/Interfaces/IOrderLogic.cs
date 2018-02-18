using OrderCenterClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterClient.Interfaces
{
    public interface IOrderLogic
    {
        List<OrderDetails> GetOrderList(DateTime affectDate, int mealTypeId, int mealId, int areaId, string bedNumber);

        string GenerateMealOrder(MealOrder mealOrder);

        List<SalesData> GetSalesData();
    }
}
