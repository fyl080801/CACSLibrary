using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ProxyContext
    {
        object _target;
        IMethodCallMessage _request;
        IMethodReturnMessage _response;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="request"></param>
        public ProxyContext(object target, IMethodCallMessage request)
        {
            _target = target;
            _request = request;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Target
        {
            get { return _target; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IMethodCallMessage Request
        {
            get { return _request; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IMethodReturnMessage Response
        {
            get { return _response; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void ThrowReturnError(Exception ex)
        {
            this._response = new ReturnMessage(ex, this.Request);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReturnNull()
        {
            this.ReturnValue(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void ReturnValue(object value)
        {
            this._response = new ReturnMessage(value, this.Response.OutArgs, this.Response.OutArgCount, this.Response.LogicalCallContext, this.Request);
        }

        internal void ReturnMessage(ReturnMessage message)
        {
            this._response = message;
        }
    }
}
