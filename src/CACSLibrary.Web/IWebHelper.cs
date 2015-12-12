using System;
using System.Runtime.InteropServices;

namespace CACSLibrary.Web
{
    public interface IWebHelper
    {
        string GetCurrentIpAddress();
        string GetStoreHost();
        string GetThisPageUrl(bool includeQueryString);
        string MapPath(string path);
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");
        string ServerVariables(string name);
    }
}

