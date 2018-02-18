using OrderCenterClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly IPaymentMethodLogic _paymentMethodLogic;
        private readonly ICache _cache;

        public PaymentMethodController(IPaymentMethodLogic paymentMethodLogic, ICache cache)
        {
            _paymentMethodLogic = paymentMethodLogic;
            _cache = cache;
        }

        // GET: PaymentMethod
        public ActionResult TogglePaymentMethodStatus(int paymentMethodTypeId, bool paymentMethodStatus)
        {
            return Json(new { UpdatePaymentMethodStatusScussfully = _paymentMethodLogic.TogglePaymentMethodStatus(paymentMethodTypeId, paymentMethodStatus) });
        }

        [HttpGet]
        public ActionResult GetPaymentMethodStatus()
        {
            return Json(new { PaymentMethods = _paymentMethodLogic.GetPaymentMethods() });
        }
    }
}