using System;
using System.Text;

namespace CACSLibrary.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginException : CACSException
    {
        string _pluginId;
        int _pluginError;
        string _message;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        public PluginException(string id, int error, string msg)
            : this(id, error, msg, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="error"></param>
        /// <param name="msg"></param>
        /// <param name="inner"></param>
        public PluginException(string id, int error, string msg, Exception inner)
            : base(msg, inner)
        {
            this._pluginId = id;
            this._pluginError = error;
            this._message = msg;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PluginId
        {
            get { return this._pluginId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PluginErrors PluginError
        {
            get { return (PluginErrors)this._pluginError; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Message
        {
            get
            {
                string value;
                switch (this.PluginError)
                {
                    case PluginErrors.Install:
                        value = string.Format("{0}, 安装异常", this.PluginId);
                        break;
                    case PluginErrors.Uninstall:
                        value = string.Format("{0}, 卸载异常", this.PluginId);
                        break;
                    case PluginErrors.Description:
                        value = string.Format("{0}, 插件描述文件异常", this.PluginId);
                        break;
                    default:
                        value = string.Format("{0}, 插件异常", this.PluginId);
                        break;
                }
                StringBuilder stringBuilder = new StringBuilder(value);
                stringBuilder.AppendLine(this._message);
                return stringBuilder.ToString();
            }
        }
    }
}
