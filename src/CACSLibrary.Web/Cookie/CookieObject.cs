using CACSLibrary.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CACSLibrary.Web.Cookie
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CookieObject : ProfileObject
    {
        private string m_CookieName = ".CACS";
        private string m_Domain = "";
        private DateTime m_Expires = DateTime.MinValue;
        private bool m_HttpOnly = true;
        private string m_Path = "/";
        private bool m_Secure = false;
        private string m_Value = "";

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual string CookieName
        {
            get { return this.m_CookieName; }
            set { this.m_CookieName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual string Domain
        {
            get { return this.m_Domain; }
            set { this.m_Domain = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual DateTime Expires
        {
            get { return this.m_Expires; }
            set { this.m_Expires = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual bool HttpOnly
        {
            get { return this.m_HttpOnly; }
            set { this.m_HttpOnly = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual string Path
        {
            get { return ((this.m_Path.IndexOf('/') == 0) ? this.m_Path : "/"); }
            set { this.m_Path = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual bool Secure
        {
            get { return this.m_Secure; }
            set { this.m_Secure = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [CookieProperty]
        public virtual string Value
        {
            get { return this.m_Value; }
            set { this.m_Value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CookieObject()
            : base(new CookieProfileProvider())
        {

        }
    }
}
