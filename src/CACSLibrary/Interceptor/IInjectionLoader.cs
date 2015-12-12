namespace CACSLibrary.Interceptor
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInjectionLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        ICallHandler[] LoadCallHandlerByCall(string className, string methodName);
    }
}
