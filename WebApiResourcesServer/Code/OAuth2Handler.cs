using DotNetOpenAuth.OAuth2;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiResourcesServer.Code
{
    public class OAuth2Handler : DelegatingHandler
    {
        private static async Task<IPrincipal> VerifyOAuth2(HttpRequestMessage httpDetails, params string[] requiredScopes)
        {
            // for this sample where the auth server and resource server are the same site,
            // we use the same public/private key.
            var resourceServer = new ResourceServer(new StandardAccessTokenAnalyzer((RSACryptoServiceProvider)Common.Configuration.SigningCertificate.PublicKey.Key, (RSACryptoServiceProvider)Common.Configuration.EncryptionCertificate.PrivateKey));
            return await resourceServer.GetPrincipalAsync(httpDetails, requiredScopes: requiredScopes);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == "Bearer")
            {
                try
                {
                    var principal = VerifyOAuth2(request);
                    if (principal.Result != null)
                    {
                        HttpContext.Current.User = principal.Result;
                        Thread.CurrentPrincipal = principal.Result;
                    }
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

               
               
                
            }

            return base.SendAsync(request, cancellationToken);
        }

    }
}