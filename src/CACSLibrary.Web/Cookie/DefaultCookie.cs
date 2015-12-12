using CACSLibrary.Profile;
using System;

namespace CACSLibrary.Web.Cookie
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultCookie : CookieObject
    {
        private string m_Content = "CACS";

        /// <summary>
        /// 
        /// </summary>
        public DefaultCookie()
        {
            base.CookieName = ".CACSDEFAULT";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Content
        {
            get { return this.m_Content; }
            set { this.m_Content = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ProfileObject GetDefault()
        {
            return new DefaultCookie()
            {
                Content = "CACS"
            };
        }
    }
}

