using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderCenterSAL
{
    class ProxyInterceptor : IInterceptor
    {
        private Func<MethodInfo,object[], object> _callback;

        public ProxyInterceptor(Func<MethodInfo, object[], object> callback)
        {
            _callback = callback;
        }

        public void Intercept(IInvocation invocation)
        {
            if (_callback != null)
            {
                invocation.ReturnValue = _callback(invocation.Method, invocation.Arguments);
            }
            else
            {
                invocation.ReturnValue = "Exception";
            }

            //throw new NotImplementedException();
        }
    }
}
