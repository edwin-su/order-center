using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Castle.DynamicProxy;
using Autofac.Extras.DynamicProxy;

namespace OrderCenterSAL
{
    public class OrderCenterSALModule : Autofac.Module
    {
        private IEnumerable<Type> _type;

        private Func<MethodInfo,object[], object> _callback;

        public OrderCenterSALModule(IEnumerable<Type> type, Func<MethodInfo, object[], object> callback)
        {
            _type = type;
            _callback = callback;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ProxyInterceptor(_callback))
                .Named<IInterceptor>("MyInterceptor").As<IInterceptor>();

            //builder.RegisterType<ProxyInterceptor>().As<IInterceptor>();
            if (_type != null)
            {
                foreach (var type in _type)
                {
                    builder.RegisterType(type)
                        .EnableClassInterceptors()
                        .InterceptedBy(typeof(IInterceptor))
                        .SingleInstance();
                }
            }
        }
    }
}
