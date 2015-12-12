using CACSLibrary.Infrastructure;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace CACSLibrary.WCF.Endpoint
{
    public class EngineInstanceProvider : IInstanceProvider
    {
        private Type _serviceContractType;
        public EngineInstanceProvider(Type serviceContractType)
        {
            this._serviceContractType = serviceContractType;
        }
        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return EngineContext.Current.Resolve(_serviceContractType);
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
