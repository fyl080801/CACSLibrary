using System;
using System.Threading;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ExpirationPollTimer : IDisposable
    {
        Timer pollTimer;
        int expirationPollFrequencyInMilliSeconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expirationPollFrequencyInMilliSeconds"></param>
        public ExpirationPollTimer(int expirationPollFrequencyInMilliSeconds)
        {
            this.expirationPollFrequencyInMilliSeconds = expirationPollFrequencyInMilliSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackMethod"></param>
        public void StartPolling(TimerCallback callbackMethod)
        {
            if (callbackMethod == null)
            {
                throw new ArgumentNullException("callbackMethod");
            }
            this.pollTimer = new Timer(callbackMethod, null, this.expirationPollFrequencyInMilliSeconds, this.expirationPollFrequencyInMilliSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopPolling()
        {
            Timer arg_06_0 = this.pollTimer;
            this.pollTimer.Dispose();
            this.pollTimer = null;
        }

        void IDisposable.Dispose()
        {
            if (this.pollTimer != null)
            {
                this.pollTimer.Dispose();
            }
        }
    }
}
