using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using IdefavOAuth2Client.Service1Reference;

namespace IdefavOAuth2Client
{
    public partial class _Default : Page
    {
        //private static readonly WebServerClient Client;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Client = new WebServerClient(authServerDescription, "sampleconsumer", "samplesecret");
            }
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            var authServer = new AuthorizationServerDescription()
            {
                
                TokenEndpoint = new Uri("http://localhost:53022/OAuth/token "),
                ProtocolVersion = ProtocolVersion.V20
            };
            WebServerClient Client= new WebServerClient(authServer, "idefav", "1");

            var code =await Client.GetClientAccessTokenAsync(new string[] { "http://localhost:55045/IService1/DoWork" });
            string token = code.AccessToken;
            Service1Reference.Service1Client service1Client=new Service1Client();
            var httpRequest = (HttpWebRequest)WebRequest.Create(service1Client.Endpoint.Address.Uri);
            ClientBase.AuthorizeRequest(httpRequest,token);
            var httpDetails = new HttpRequestMessageProperty();
            httpDetails.Headers[HttpRequestHeader.Authorization] = httpRequest.Headers[HttpRequestHeader.Authorization];
            
            using (var scope = new OperationContextScope(service1Client.InnerChannel))
            {
                
                if (OperationContext.Current.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpDetails;
                }
                else
                {
                    OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, httpDetails);
                }
                
                Button1.Text= service1Client.DoWork();
            }


        }
    }
}