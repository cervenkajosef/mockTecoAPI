using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace mockTecoAPI
{
    public class CertGen
    {
        public X509Certificate2 GenerateSelfSignedCertificate()
        {
            using (RSA rsa = RSA.Create())
            {
                var certificateRequest = new CertificateRequest(
                    new X500DistinguishedName("CN=YourSelfSignedCertificate"),
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                var certificate = certificateRequest.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(365));

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx));
            }
        }

    }
}
