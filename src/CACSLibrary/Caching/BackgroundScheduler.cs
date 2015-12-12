using System;
using System.Threading;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundScheduler : ICacheScavenger
    {
        ExpirationTask expirationTask;
        ScavengerTask scavengerTask;
        object scavengeExpireLock = new object();
        int scavengePending;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expirationTask"></param>
        /// <param name="scavengerTask"></param>
        public BackgroundScheduler(ExpirationTask expirationTask, ScavengerTask scavengerTask)
        {
            this.expirationTask = expirationTask;
            this.scavengerTask = scavengerTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notUsed"></param>
        public void ExpirationTimeoutExpired(object notUsed)
        {
            ThreadPool.QueueUserWorkItem(delegate(object unused)
            {
                this.BackgroundWork(new Action(this.Expire));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartScavenging()
        {
            int num = Interlocked.Increment(ref this.scavengePending);
            if (num == 1)
            {
                ThreadPool.QueueUserWorkItem(delegate(object unused)
                {
                    this.BackgroundWork(new Action(this.Scavenge));
                });
            }
        }

        internal void StartScavengingIfNeeded()
        {
            if (this.scavengerTask.IsScavengingNeeded())
            {
                this.StartScavenging();
            }
        }

        internal void BackgroundWork(Action work)
        {
            try
            {
                lock (this.scavengeExpireLock)
                {
                    work();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void Scavenge()
        {
            int num = Interlocked.Exchange(ref this.scavengePending, 0);
            for (int i = (num - 1) / this.scavengerTask.NumberOfItemsToBeScavenged + 1; i > 0; i--)
            {
                this.scavengerTask.DoScavenging();
            }
        }

        internal void Expire()
        {
            this.expirationTask.DoExpirations();
        }
    }
}
