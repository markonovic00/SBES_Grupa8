using Manager;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace LocalDatabase
{
    class ClientProxy : ChannelFactory<ICentral>, ICentral, IDisposable
    {
        ICentral factory;
        public static ClientProxy proxy { get; set; }
        

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            string cltCetCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                 System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                new ClientCertValidator();

            this.Credentials.ServiceCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate =
                CertManager.GetCertificateFromStorage(System.Security.Cryptography.X509Certificates.StoreName.My
                ,System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                cltCetCN);

            factory = this.CreateChannel();
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCetCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

            this.Credentials.ServiceCertificate.Authentication.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate =
                CertManager.GetCertificateFromStorage(System.Security.Cryptography.X509Certificates.StoreName.My
                , System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                cltCetCN);

            factory = this.CreateChannel();
            //Credentials.Windows.AllowNtlm = false;
        }

        public int Ping()
        {
            try
            {
                return factory.Ping();
            }
            catch (Exception e)
            {                
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int checkMatching(List<Data> localData)
        {
            try
            {
                return factory.checkMatching(localData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int writeCentral(Data localData)
        {
            try
            {
                return factory.writeCentral(localData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int updateConsumpion(string region, string city, double value)
        {
            try
            {
                return factory.updateConsumpion(region, city, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int deleteData(Data _data)
        {
            try
            {
                return factory.deleteData(_data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int updateRecords(List<Data> _data)
        {
            try
            {
                return factory.updateRecords(_data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }
    }
}
