using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace WCFRescourcesServer.Code
{
    public class ResourceServerConfiguration
    {
        public X509Certificate2 EncryptionCertificate { get; set; }
        public X509Certificate2 SigningCertificate { get; set; }
    }
}