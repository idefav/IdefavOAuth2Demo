using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

namespace WebformResourcesServer.Code
{
    public class AshxHandler
    {
        public AshxHandler(HttpContext context)
        {
            Context = context;
        }

        public HttpContext Context { get; set; }

        private async Task<IPrincipal> VerifyOAuth2(HttpRequestBase httpDetails, params string[] requiredScopes)
        {
            var resourceServer = new ResourceServer(new StandardAccessTokenAnalyzer((RSACryptoServiceProvider)Common.Configuration.SigningCertificate.PublicKey.Key, (RSACryptoServiceProvider)Common.Configuration.EncryptionCertificate.PrivateKey));
            return await resourceServer.GetPrincipalAsync(httpDetails, requiredScopes: requiredScopes);
           
        }

        public async Task Proc(Action<HttpContext> action)
        {
            try
            {
                var principal = await VerifyOAuth2(new HttpRequestWrapper(Context.Request));
                if (principal != null)
                {
                    Context.User = principal;
                    Thread.CurrentPrincipal = principal;
                    action.Invoke(Context);
                }
            }
            catch (ProtocolFaultResponseException exception)
            {
                var outgoingResponse = await exception.CreateErrorResponseAsync(CancellationToken.None);
                Context.Response.StatusCode = (int)outgoingResponse.StatusCode;
                //Context.Response.SuppressContent = true;
                foreach (var header in outgoingResponse.Headers)
                {

                    //Context.Response.Headers[header.Key] = header.Value.First();
                    Context.Response.AddHeader(header.Key, header.Value.First());
                }
                Context.Response.Write(exception.Message);
            }
        }
    }
}