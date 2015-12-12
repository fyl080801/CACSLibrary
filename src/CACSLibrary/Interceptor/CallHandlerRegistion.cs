using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public class CallHandlerRegistion
    {
        ICallHandler _callHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="callHandler"></param>
        public CallHandlerRegistion(string typeName, ICallHandler callHandler)
        {
            this._callHandler = callHandler;
            this.TypeName = typeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <param name="callHandler"></param>
        public CallHandlerRegistion(string typeName, string methodName, ICallHandler callHandler)
            : this(typeName, callHandler)
        {
            this.MethodName = methodName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <param name="index"></param>
        /// <param name="callHandler"></param>
        public CallHandlerRegistion(string typeName, string methodName, int index, ICallHandler callHandler)
            : this(typeName, methodName, callHandler)
        {
            this.Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return this._callHandler.Index; }
            set { this._callHandler.Index = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICallHandler CallHandler
        {
            get { return _callHandler; }
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
    }
}
