using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public class CallHandlerPipeline
    {
        object _target;
        IList<ICallHandler> _callhandlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public CallHandlerPipeline(object target)
            : this(new List<ICallHandler>(), target)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callhandlers"></param>
        /// <param name="target"></param>
        public CallHandlerPipeline(IList<ICallHandler> callhandlers, object target)
        {
            if (callhandlers == null)
                throw new ArgumentNullException("callhandlers");
            if (target == null)
                throw new ArgumentNullException("target");
            this._target = target;
            this._callhandlers = callhandlers;
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
        /// <param name="context"></param>
        public void Invoke(ProxyContext context)
        {
            Stack<object> objectStack = new Stack<object>();
            Stack<ICallHandler> handlerStack = new Stack<ICallHandler>();
            foreach (ICallHandler current in this._callhandlers)
            {
                try
                {
                    objectStack.Push(current.PreInvoke(context));
                    if (context.Response != null && context.Response.Exception != null && !current.IgnoreCallHandlerException)
                    {
                        context.ReturnMessage(new ReturnMessage(context.Response.Exception, context.Request));
                        return;
                    }
                }
                catch (Exception callhandlerex)
                {
                    if (!current.IgnoreCallHandlerException)
                    {
                        context.ReturnMessage(new ReturnMessage(callhandlerex.InnerException ?? callhandlerex, context.Request));
                        return;
                    }
                    object item = null;
                    objectStack.Push(item);
                }
                handlerStack.Push(current);
            }
            object[] array = Array.CreateInstance(typeof(object), context.Request.Args.Length) as object[];
            context.Request.Args.CopyTo(array, 0);
            try
            {
                object ret = context.Request.MethodBase.Invoke(this._target, array);
                context.ReturnMessage(new ReturnMessage(ret, array, array.Length, context.Request.LogicalCallContext, context.Request));
            }
            catch (Exception methodex)
            {
                context.ReturnMessage(new ReturnMessage(methodex.InnerException ?? methodex, context.Request));
            }
            while (handlerStack.Count > 0)
            {
                ICallHandler callHandler = handlerStack.Pop();
                object objState = objectStack.Pop();
                if (context.Response == null || context.Response.Exception == null || callHandler.IgnoreMethodException)
                {
                    callHandler.AftInvoke(context, objState);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callhandler"></param>
        public void Add(ICallHandler callhandler)
        {
            if (callhandler == null)
            {
                throw new ArgumentNullException("callhandler");
            }
            this._callhandlers.Add(callhandler);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this._callhandlers.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callhandler"></param>
        public void Remove(ICallHandler callhandler)
        {
            if (callhandler == null)
            {
                throw new ArgumentNullException("callhandler");
            }
            if (this._callhandlers.Contains(callhandler))
            {
                this._callhandlers.Remove(callhandler);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this._callhandlers.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICallHandler> GetCallHandlers()
        {
            return this._callhandlers.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Sort()
        {
            ICallHandler[] array = this._callhandlers.ToArray<ICallHandler>();
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i].Index > array[j].Index)
                    {
                        ICallHandler callHandler = array[i];
                        array[i] = array[j];
                        array[j] = callHandler;
                    }
                }
            }
            this._callhandlers = array.ToList<ICallHandler>();
        }
    }
}
