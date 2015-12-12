using System;

namespace CACSLibrary.Web.Cookie
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CookiePropertyAttribute : Attribute
    {
        private bool m_IsCookieMark = true;

        public bool IsCookieMark
        {
            get
            {
                return this.m_IsCookieMark;
            }
            set
            {
                this.m_IsCookieMark = value;
            }
        }

        public CookieObject CookieObject
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}

