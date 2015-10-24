using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.OAuth2.ChannelElements;
using DotNetOpenAuth.OAuth2.Messages;

namespace IdefavAuthorizationServer.Code
{
    public class IdefavAuthorizationServerHost : IAuthorizationServerHost
    {
        /// <summary>
        /// 配置
        /// </summary>
        private readonly AuthorizationServerConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public IdefavAuthorizationServerHost(AuthorizationServerConfiguration config)
        {
            if (config != null)
                _configuration = config;
        }

        /// <summary>
        /// Token创建
        /// </summary>
        /// <param name="accessTokenRequestMessage"></param>
        /// <returns></returns>
        public AccessTokenResult CreateAccessToken(IAccessTokenRequest accessTokenRequestMessage)
        {
            var accessToken = new AuthorizationServerAccessToken();
            accessToken.Lifetime = _configuration.TokenLifetime;//设置Token的有效时间

            // 设置加密公钥
            accessToken.ResourceServerEncryptionKey =
                (RSACryptoServiceProvider)_configuration.EncryptionCertificate.PublicKey.Key;
            // 设置签名私钥
            accessToken.AccessTokenSigningKey = (RSACryptoServiceProvider)_configuration.SigningCertificate.PrivateKey;

            var result = new AccessTokenResult(accessToken);
            return result;
        }

        public IClientDescription GetClient(string clientIdentifier)
        {
            // 这里需要去验证客户端发送过来的client_id
            if (string.Equals(clientIdentifier, "idefav", StringComparison.CurrentCulture))// 这里为了简明起见没有使用数据库
            {
                var client=new Client
                {
                    Name = "idefav",
                    ClientSecret = "1",
                    ClientType = 1
                };
                return client;
            }
            throw new ArgumentOutOfRangeException("clientIdentifier");
        }

        public bool IsAuthorizationValid(IAuthorizationDescription authorization)
        {
            return true;
        }

        public AutomatedUserAuthorizationCheckResponse CheckAuthorizeResourceOwnerCredentialGrant(string userName, string password,
            IAccessTokenRequest accessRequest)
        {
            throw new NotImplementedException();
        }

        public AutomatedAuthorizationCheckResponse CheckAuthorizeClientCredentialsGrant(IAccessTokenRequest accessRequest)
        {
            AutomatedUserAuthorizationCheckResponse response = new AutomatedUserAuthorizationCheckResponse(accessRequest, true, "test");
            return response;
        }

        public ICryptoKeyStore CryptoKeyStore { get; }
        public INonceStore NonceStore { get; }

       
    }
}