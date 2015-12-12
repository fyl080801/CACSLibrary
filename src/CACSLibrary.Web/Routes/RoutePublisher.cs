using CACSLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace CACSLibrary.Web.Routes
{
    public class RoutePublisher : IRoutePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public RoutePublisher(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            IEnumerable<Type> enumerable = this._typeFinder.FindClassesOfType<IRouteProvider>(true);
            List<IRouteProvider> list = new List<IRouteProvider>();
            foreach (Type type in enumerable)
            {
                IRouteProvider item = Activator.CreateInstance(type) as IRouteProvider;
                list.Add(item);
            }
            (from rp in list
                orderby rp.Priority descending
                select rp).ToList<IRouteProvider>().ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}

