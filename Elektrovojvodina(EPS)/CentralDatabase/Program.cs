using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.ServiceModel.Security;
using System.ServiceModel.Description;

namespace CentralDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/CentralService";

            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(Central));
            host.AddServiceEndpoint(typeof(ICentral), binding, address);

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
                new ServiceCertValidator();

            host.Credentials.ClientCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

            host.Credentials.ServiceCertificate.Certificate =
                CertManager.GetCertificateFromStorage(System.Security.Cryptography.X509Certificates.StoreName.My,
                System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                srvCertCN);

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();

            Console.WriteLine("Korisnik koji je pokrenuo servera :" + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("Servis je pokrenut.");

            Console.ReadLine();
        }
    }
}
