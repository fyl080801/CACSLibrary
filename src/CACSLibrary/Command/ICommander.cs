using System;

namespace CACSLibrary.Command
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommander
    {
        /// <summary>
        /// 
        /// </summary>
        object[] Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        string CommandName
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        void Execute();
    }
}
