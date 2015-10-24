using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using WebformResourcesServer.Code;

namespace WebformResourcesServer
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            Common.Configuration = new ResourceServerConfiguration
            {
                EncryptionCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.pfx"), "a"),
                SigningCertificate = new X509Certificate2(Server.MapPath("~/Certs/idefav.cer"))
            };
            // 在应用程序启动时运行的代码
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}