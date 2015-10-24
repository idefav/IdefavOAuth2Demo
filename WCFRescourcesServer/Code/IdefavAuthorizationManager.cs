using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using ProtocolException = System.ServiceModel.ProtocolException;

namespace WCFRescourcesServer.Code
{
    public class IdefavAuthorizationManager : ServiceAuthorizationManager
    {
        public IdefavAuthorizationManager()
        {
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (!base.CheckAccessCore(operationContext))
            {
                return false;
            }

            var httpDetails = operationContext.RequestContext.RequestMessage.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            var requestUri = operationContext.RequestContext.RequestMessage.Properties.Via;

            return Task.Run(async delegate
            {
                ProtocolFaultResponseException exception = null;
                try
                {
                    var principal = await VerifyOAuth2Async(
                        httpDetails,
                        requestUri,
                        operationContext.IncomingMessageHeaders.Action ?? operationContext.IncomingMessageHeaders.To.AbsolutePath);
                    if (principal != null)
                    {
                        var policy = new OAuthPrincipalAuthorizationPolicy(principal);
                        var policies = new List<IAuthorizationPolicy> { policy };

                        var securityContext = new ServiceSecurityContext(policies.AsReadOnly());
                        if (operationContext.IncomingMessageProperties.Security != null)
                        {
                            operationContext.IncomingMessageProperties.Security.ServiceSecurityContext = securityContext;
                        }
                        else
                        {
                            operationContext.IncomingMessageProperties.Security = new SecurityMessageProperty
                            {
                                ServiceSecurityContext = securityContext,
                            };
                        }

                        securityContext.AuthorizationContext.Properties["Identities"] = new List<IIdentity> { principal.Identity, };

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (ProtocolFaultResponseException ex)
                {

                    exception = ex;
                }
                catch (ProtocolException ex)
                {

                }

                if (exception != null)
                {
                    
                    // Return the appropriate unauthorized response to the client.
                    var outgoingResponse = await exception.CreateErrorResponseAsync(CancellationToken.None);
                    if (WebOperationContext.Current != null)
                        this.Respond(WebOperationContext.Current.OutgoingResponse, outgoingResponse);
                }

                return false;
            }).GetAwaiter().GetResult();
        }

        private static async Task<IPrincipal> VerifyOAuth2Async(HttpRequestMessageProperty httpDetails, Uri requestUri, params string[] requiredScopes)
        {
            var resourceServer = new ResourceServer(new StandardAccessTokenAnalyzer((RSACryptoServiceProvider)Common.Configuration.SigningCertificate.PublicKey.Key, (RSACryptoServiceProvider)Common.Configuration.EncryptionCertificate.PrivateKey));
            return await resourceServer.GetPrincipalAsync(httpDetails, requestUri, requiredScopes: requiredScopes);
        }

        private void Respond(OutgoingWebResponseContext responseContext, HttpResponseMessage responseMessage)
        {
            responseContext.StatusCode = responseMessage.StatusCode;
            responseContext.SuppressEntityBody = true;
            foreach (var header in responseMessage.Headers)
            {
                responseContext.Headers[header.Key] = header.Value.First();
            }
        }
    }
}