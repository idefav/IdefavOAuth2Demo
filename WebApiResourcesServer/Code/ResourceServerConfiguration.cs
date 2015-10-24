using System.Security.Cryptography.X509Certificates;

namespace WebApiResourcesServer.Code
{
    public class ResourceServerConfiguration
    {
        public X509Certificate2 EncryptionCertificate { get; set; }
        public X509Certificate2 SigningCertificate { get; set; }
    }
}