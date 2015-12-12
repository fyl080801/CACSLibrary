namespace CACSLibrary.Web.Routes
{
    using System;
    using System.Web.Routing;

    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routes);
    }
}

