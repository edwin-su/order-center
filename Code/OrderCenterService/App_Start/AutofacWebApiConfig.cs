using Autofac;
using Autofac.Integration.WebApi;
using OrderCenterService.Interfaces;
using OrderCenterService.Logics;
using OrderCenterService.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace OrderCenterService.App_Start
{
    public static class AutofacWebApiConfig
    {
        public static void Run()
        {
            SetAutofacWebApi();
        }

        private static void SetAutofacWebApi()
        {
            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UserLogic>().As<IUserLogic>();
            builder.RegisterType<CacheStore>().As<ICache>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}