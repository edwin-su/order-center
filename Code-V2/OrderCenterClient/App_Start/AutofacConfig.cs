﻿using Autofac;
using Autofac.Integration.Mvc;
using OrderCenterClient.Filters;
using OrderCenterClient.Interfaces;
using OrderCenterClient.Logics;
using OrderCenterClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OrderCenterClient
{
    public class AutofacConfig
    {
        public static void Run()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<HomeLogic>().As<IHomeLogic>();
            builder.RegisterType<CacheStore>().As<ICache>();
            builder.RegisterType<MealLogic>().As<IMealLogic>();
            builder.RegisterType<PaymentMethodLogic>().As<IPaymentMethodLogic>();
            builder.RegisterType<OrderLogic>().As<IOrderLogic>();

            builder.Register(c => new AuthorizationFilter(
                  c.Resolve<ICache>()))
                  .AsAuthorizationFilterFor<Controller>()
                  .InstancePerHttpRequest();

            builder.Register(c=>new CustomExceptionFilter()).AsExceptionFilterFor<Controller>()
                .InstancePerHttpRequest();

            builder.Register(c => new JsonBehaviorFilter()).AsActionFilterFor<Controller>()
       .InstancePerHttpRequest();

            builder.RegisterFilterProvider();


            var container = builder.Build();

            // Configure MVC with the dependency resolver.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            // Configure MVC move the version header
            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}