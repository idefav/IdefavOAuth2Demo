using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using IdefavAuthorizationServer.Code;

namespace IdefavAuthorizationServer.Controllers
{
    public class OAuthController : Controller
    {
        private readonly AuthorizationServer authorizationServer =
           new AuthorizationServer(new IdefavAuthorizationServerHost(Common.Configuration));
        
        public async Task<ActionResult> Token()
        {
            var response = await authorizationServer.HandleTokenRequestAsync(Request);
            Response.ContentType = response.Content.Headers.ContentType.ToString();
            return response.AsActionResult();
        }
    }
}