using OrderCenterInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderCenterInterface.Controllers
{
    public abstract class AbstractUserController : BaseController
    {
        [Route("User/Login")]
        [HttpPost]
        public abstract long Login(OrderCredential credential);
    }
}
