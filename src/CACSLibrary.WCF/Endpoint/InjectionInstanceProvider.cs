using CACSLibrary.Interceptor;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace CACSLibrary.WCF.Endpoint
{
    public class InjectionInstanceProvider : IInstanceProvider
    {
        private Type _serviceContractType;
        public InjectionInstanceProvider(Type serviceContractType)
        {
            this._serviceContractType = serviceContractType;
        }
        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            Type serviceType = instanceContext.Host.Description.ServiceType;
            object instance = Activator.CreateInstance(serviceType);
            if (!(this._serviceContractType.IsInterface || serviceType.IsMarshalByRef))
            {
                return instance;
            }
            return Injection.Wrap(this._serviceContractType, instance, new IInterceptor[0]);
        }
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}