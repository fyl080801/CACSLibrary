using System;
using System.Collections.Generic;

namespace CACSLibrary.Command
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseCommanderProvider : ICommanderProvider
    {
        Dictionary<string, ICommander> _Commands;

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, ICommander> Commands
        {
            get
            {
                if (this._Commands == null)
                {
                    this._Commands = new Dictionary<string, ICommander>();
                }
                return this._Commands;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        public virtual void AddCommand(string key, ICommander command)
        {
            this.Commands.Add(key, command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveCommand(string key)
        {
            if (this.Commands.ContainsKey(key))
            {
                this.Commands.Remove(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ClearCommand()
        {
            this.Commands.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        public abstract void ExecuteCommand(string cmdstr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        public abstract void ExecuteChain(string cmdstr);
    }
}
