using System;

namespace CACSLibrary.Command
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommanderProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        void AddCommand(string key, ICommander command);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void RemoveCommand(string key);

        /// <summary>
        /// 
        /// </summary>
        void ClearCommand();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        void ExecuteCommand(string cmdstr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        void ExecuteChain(string cmdstr);
    }
}
