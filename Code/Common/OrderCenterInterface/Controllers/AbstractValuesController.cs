using OrderCenterInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderCenterInterface.Controllers
{
    public abstract class AbstractValuesController : BaseController
    {
        [Route("Values/Get/{id}")]
        [HttpGet]
        public abstract string Get(int id);

        [Route("Values/Post")]
        [HttpPost]
        public abstract void Post(OrderUser user);
    }
}
