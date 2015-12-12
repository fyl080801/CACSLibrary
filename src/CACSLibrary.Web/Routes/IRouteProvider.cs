namespace CACSLibrary.Web.Routes
{
    using System;
    using System.Web.Routing;

    public interface IRouteProvider
    {
        void RegisterRoutes(RouteCollection routes);

        int Priority { get; }
    }
}

