using System;
using System.Runtime.Serialization;

namespace CACSLibrary
{
    /// <summary>
    /// 异常类
    /// </summary>
    public class CACSException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CACSException()
        {
        }

        /// <summary>
        /// 以消息明细构建异常
        /// </summary>
        /// <param name="message">异常消息</param>
        public CACSException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public CACSException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 以序列化消息构建异常
        /// </summary>
        /// <param name="info">消息</param>
        /// <param name="context">序列化流上下文</param>
        public CACSException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
