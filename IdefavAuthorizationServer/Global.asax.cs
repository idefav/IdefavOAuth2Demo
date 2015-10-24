using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IdefavAuthorizationServer.Code;

namespace IdefavAuthorizationServer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Common.Configuration=new AuthorizationServerConfiguration
            {
                EncryptionCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.cer")),
                SigningCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.pfx"),"a")
            };
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
