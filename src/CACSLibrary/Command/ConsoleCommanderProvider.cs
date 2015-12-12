using System;

namespace CACSLibrary.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class ConsoleCommanderProvider : BaseCommanderProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        public override void AddCommand(string key, ICommander command)
        {
            if (command is ConsoleCommander)
            {
                ((ConsoleCommander)command).Provider = this;
                base.AddCommand(key, command);
                return;
            }
            throw new ArgumentException("必须是ConsoleCommand的派生类", "command");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        public override void ExecuteCommand(string cmdstr)
        {
            if (!base.Commands.ContainsKey(cmdstr))
            {
                Console.WriteLine(string.Format("[{0}] - 未知命令", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
                return;
            }
            ICommander commander = base.Commands[cmdstr];
            Console.WriteLine(string.Format("[{0}] - {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), commander.CommandName));
            commander.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdstr"></param>
        public override void ExecuteChain(string cmdstr)
        {
            this.ExecuteCommand(cmdstr);
            if (base.Commands.Count > 0)
            {
                string cmdstr2 = Console.ReadLine();
                this.ExecuteChain(cmdstr2);
            }
        }
    }
}
