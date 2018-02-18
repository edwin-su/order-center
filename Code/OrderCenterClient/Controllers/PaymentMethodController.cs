using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient.Controllers
{
    public class PaymentMethodController : Controller
    {
        // GET: PaymentMethod
        public ActionResult TogglePaymentMethodStatus(int paymentMethodType, bool paymentMethodStatus)
        {
            paymentMethodStatus = !paymentMethodStatus;
            //var cardHolderId = _senderLogic.GetSenderId();

            return Json(new { PaymentMethodType = paymentMethodType, PaymentMethodStatus = paymentMethodStatus });
        }
    }
}