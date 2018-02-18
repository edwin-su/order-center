using OrderCenterInterface.Controllers;
using OrderCenterInterface.Models;
using OrderCenterService.Interfaces;
using OrderCenterService.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderCenterService.Controllers
{
    public class ValuesController : AbstractValuesController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private readonly IUserLogic _userLogic;
        private readonly ICache _cache;

        public ValuesController(IUserLogic userLogic, ICache cache)
        {
            _userLogic = userLogic;
            _cache = cache;
        }

        [Route("Values/Get/{id}")]
        public override string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [Route("Values/Post")]
        public override void Post(OrderUser user)
        {

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
