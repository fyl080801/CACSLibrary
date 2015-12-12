using CACSLibrary.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyLoader : IInjectionLoader
    {
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
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public ICallHandler[] LoadCallHandlerByCall(string className, string methodName)
        {
            IList<ICallHandler> list = new List<ICallHandler>();
            var registions = EngineContext.Current.ResolveAll<CallHandlerRegistion>();
            foreach (var registion in registions)
            {
                if ((registion.TypeName == className && registion.MethodName == null) || (registion.TypeName == className && registion.MethodName == methodName))
                {
                    list.Add(registion.CallHandler);
                }
            }
            return list.ToArray();
        }
    }
}
