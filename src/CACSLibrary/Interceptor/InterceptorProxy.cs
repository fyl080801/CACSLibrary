using CACSLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public class InterceptorProxy : RealProxy
    {
        List<ICallHandler> _callhandlers;
        object _target;
        IInjectionLoader _loader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        /// <param name="callhandlers"></param>
        public InterceptorProxy(Type type, object target, params ICallHandler[] callhandlers)
            : base(type)
        {
            this._target = target;
            this._callhandlers = new List<ICallHandler>();
            if (callhandlers != null)
            {
                this._callhandlers.AddRange(callhandlers);
            }

            var config = ConfigurationManager.GetSection("injectionConfig") as InjectionConfig;
            if (config == null || config.loaderType == null || string.IsNullOrEmpty(config.loaderType))
            {
                _loader = new DependencyLoader();
            }
            else
            {
                _loader = (IInjectionLoader)Activator.CreateInstance(Type.GetType(config.loaderType));
            }
        }

        public ICallHandler ICallHandler
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCallMessage = (IMethodCallMessage)msg;
            IMethodReturnMessage result = null;
            try
            {
                ProxyContext proxyContext = new ProxyContext(this._target, methodCallMessage);
                CallHandlerPipeline callHandlerPipeline = new CallHandlerPipeline(this._target);

                var configHandlers = _loader.LoadCallHandlerByCall(methodCallMessage.MethodBase.DeclaringType.FullName, methodCallMessage.MethodName);
                foreach (var handler in configHandlers)
                {
                    callHandlerPipeline.Add(handler);
                }

                foreach (ICallHandler handler in this._callhandlers)
                {
                    callHandlerPipeline.Add(handler);
                }
                Type type = Type.GetType(methodCallMessage.TypeName);
                List<object> list = new List<object>();
                list.AddRange(type.GetCustomAttributes(typeof(IInterceptor), true));
                list.AddRange(methodCallMessage.MethodBase.GetCustomAttributes(typeof(IInterceptor), true));
                foreach (object obj in list)
                {
                    IInterceptor interceptor = obj as IInterceptor;
                    if (interceptor != null)
                    {
                        callHandlerPipeline.Add(interceptor.BuildCallHandler());
                    }
                }
                callHandlerPipeline.Sort();
                callHandlerPipeline.Invoke(proxyContext);
                result = proxyContext.Response;
            }
            catch (Exception ex)
            {
                result = new ReturnMessage(ex.InnerException ?? ex, methodCallMessage);
            }
            return result;
        }
    }
}
