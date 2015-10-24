using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

namespace IdefavAuthorizationServer.Code
{
    public class Client : IClientDescription
    {
        /// <summary>
        /// 客户端名称client_id
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户端类型
        /// </summary>
        public int ClientType { get; set; }

        /// <summary>
        /// 回调URL
        /// </summary>
        public string Callback { get; set; }

        public string ClientSecret { get; set; }


        Uri IClientDescription.DefaultCallback
        {
            get { return string.IsNullOrEmpty(this.Callback) ? null : new Uri(this.Callback); }
        }


        ClientType IClientDescription.ClientType
        {
            get { return (ClientType)this.ClientType; }
        }


        bool IClientDescription.HasNonEmptySecret
        {
            get { return !string.IsNullOrEmpty(this.ClientSecret); }
        }


        bool IClientDescription.IsCallbackAllowed(Uri callback)
        {
            if (string.IsNullOrEmpty(this.Callback))
            {
                // No callback rules have been set up for this client.
                return true;
            }

            // In this sample, it's enough of a callback URL match if the scheme and host match.
            // In a production app, it is advisable to require a match on the path as well.
            Uri acceptableCallbackPattern = new Uri(this.Callback);
            if (string.Equals(acceptableCallbackPattern.GetLeftPart(UriPartial.Authority), callback.GetLeftPart(UriPartial.Authority), StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }


        bool IClientDescription.IsValidClientSecret(string secret)
        {
            return MessagingUtilities.EqualsConstantTime(secret, this.ClientSecret);
        }


    }
}