using CACSLibrary;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Hosting;

namespace CACSLibrary.Web
{
    public class WebHelper : IWebHelper
    {
        private readonly HttpContextBase _httpContext;

        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        public string GetCurrentIpAddress()
        {
            if (((this._httpContext != null) && (this._httpContext.Request != null)) && (this._httpContext.Request.UserHostAddress != null))
            {
                return this._httpContext.Request.UserHostAddress;
            }
            return string.Empty;
        }

        public virtual string GetStoreHost()
        {
            string str = this.ServerVariables("HTTP_HOST");
            string str2 = "";
            if (!string.IsNullOrEmpty(str))
            {
                str2 = "http://" + str;
            }
            if (!str2.EndsWith("/"))
            {
                str2 = str2 + "/";
            }
            return str2.ToLowerInvariant();
        }

        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            string leftPart = string.Empty;
            if (this._httpContext == null)
            {
                return leftPart;
            }
            if (includeQueryString)
            {
                string storeHost = this.GetStoreHost();
                if (storeHost.EndsWith("/"))
                {
                    storeHost = storeHost.Substring(0, storeHost.Length - 1);
                }
                leftPart = storeHost + this._httpContext.Request.RawUrl;
            }
            else
            {
                leftPart = this._httpContext.Request.Url.GetLeftPart(UriPartial.Path);
            }
            return leftPart.ToLowerInvariant();
        }

        public virtual string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart(new char[] { '/' }).Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public virtual void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "")
        {
            if (WebCommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                HttpRuntime.UnloadAppDomain();
                this.TryWriteGlobalAsax();
            }
            else
            {
                if (!this.TryWriteWebConfig())
                {
                    throw new CACSException("重启web应用异常,无法访问 web.config");
                }
                if (!this.TryWriteGlobalAsax())
                {
                    throw new CACSException("重启web应用异常,无法访问 Global.asax");
                }
            }
            if ((this._httpContext != null) && makeRedirect)
            {
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = this.GetThisPageUrl(true);
                }
                this._httpContext.Response.Redirect(redirectUrl, true);
            }
        }

        public virtual string ServerVariables(string name)
        {
            string str = string.Empty;
            try
            {
                if (this._httpContext.Request.ServerVariables[name] != null)
                {
                    str = this._httpContext.Request.ServerVariables[name];
                }
            }
            catch
            {
                str = string.Empty;
            }
            return str;
        }

        private bool TryWriteGlobalAsax()
        {
            try
            {
                File.SetLastWriteTimeUtc(this.MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                File.SetLastWriteTimeUtc(this.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

