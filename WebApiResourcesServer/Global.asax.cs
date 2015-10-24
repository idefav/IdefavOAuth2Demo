using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApiResourcesServer.Code;

namespace WebApiResourcesServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Common.Configuration = new ResourceServerConfiguration
            {
                EncryptionCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.pfx"), "a"),
                SigningCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.cer"))
            };
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
