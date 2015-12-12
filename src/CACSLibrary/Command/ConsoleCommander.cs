using System;

namespace CACSLibrary.Command
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ConsoleCommander : ICommander
    {
        ICommanderProvider _Provider;

        /// <summary>
        /// 
        /// </summary>
        public EventHandler<EventArgs> Executed;

        /// <summary>
        /// 
        /// </summary>
        public abstract string CommandName
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual object[] Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommanderProvider Provider
        {
            get
            {
                return this._Provider;
            }
            set
            {
                this._Provider = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Execute()
        {
            this.Invoke();
            this.OnExecuted();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnExecuted()
        {
            if (this.Executed != null)
            {
                this.Executed(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Invoke();
    }
}
