using CACSLibrary.Profile;
using System;
using System.Reflection;
using System.Web;

namespace CACSLibrary.Web.Cookie
{
    public class CookieProfileProvider : IProfileProvider
    {
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
    
        public object Load(Type configType)
        {
            if (!typeof(CookieObject).IsAssignableFrom(configType))
                throw new CACSException("读取 cookie 时对象不是 cookie 类型");
            object cookie = Activator.CreateInstance(configType);
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies[((CookieObject)cookie).CookieName];
            if (httpCookie != null)
            {
                foreach (string str in httpCookie.Values.Keys)
                {
                    PropertyInfo property = cookie.GetType().GetProperty(str);
                    if (property != null)
                    {
                        object propertyObject = Convert.ChangeType(httpCookie.Values[str], property.PropertyType);
                        property.SetValue(cookie, propertyObject, null);
                    }
                }
                return cookie;
            }
            else
            {
                return ((CookieObject)cookie).GetDefault();
            }
        }

        public void Save(object config)
        {
            CookieObject obj = config as CookieObject;
            if (obj == null)
                throw new CACSException("保存 cookie 时对象不是 cookie 类型");
            HttpCookie cookie = new HttpCookie(obj.CookieName)
            {
                Domain = obj.Domain,
                Expires = obj.Expires,
                HttpOnly = obj.HttpOnly,
                Path = obj.Path,
                Secure = obj.Secure,
                Value = obj.Value
            };
            PropertyInfo[] properties = obj.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                CookiePropertyAttribute cookieAttribute = properties[i].GetCustomAttribute<CookiePropertyAttribute>(true);
                if (cookieAttribute != null && cookieAttribute.IsCookieMark)
                {
                    cookie.Values.Add(properties[i].Name, properties[i].GetValue(obj, null).ToString());
                }
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void Clear(object config)
        {
            CookieObject obj = config as CookieObject;
            if (obj == null)
                throw new CACSException("清除 cookie 时对象不是 cookie 类型");
            HttpCookie cookie = HttpContext.Current.Response.Cookies[obj.CookieName];
            if (cookie != null)
            {
                HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            }
        }
    }
}
