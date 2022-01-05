using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceContracts;
using System.Security.Principal;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Manager;

namespace LocalDatabase
{
    class Program
    {
        
        static bool IsBusy(int port)
        {
            IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();
            if (endpoints == null || endpoints.Length == 0) return false;
            for (int i = 0; i < endpoints.Length; i++)
                if (endpoints[i].Port == port)
                    return true;
            return false;
        }

        static void Main(string[] args)
        {
            // problem pri kreiranju vise lokalnih baza
            // definisanje regiona nad kojem rade
            LocalSettings localSettings = new LocalSettings();
            var item = new KeyValuePair<int, string>();
            for (int i = 0; i < localSettings.Ports.Count; i++)
            {
                item = localSettings.Ports.ElementAt(i);
                Console.WriteLine(i + ". " + item.Value);
            }
            // provera da li je port slobodan
            while (true) 
            { 
                Console.WriteLine("Molimo vas unesite region: ");
                int region = Convert.ToInt32(Console.ReadLine());
                item = localSettings.Ports.ElementAt(region);
                
                if (!IsBusy(item.Key))
                    break;
            }

            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:"+item.Key+"/"+item.Value;
            
            // Kreira se adresa sa svakim regionom, potrebno je na klijnetu isto to odraditit

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(Local));
            host.AddServiceEndpoint(typeof(ILocal), binding, address);

            host.Open();

            Console.WriteLine("Korisnik koji je pokrenuo Lokalnogservera :" + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("Servis je pokrenut.");

            #region centralConnection

            string srvCertName = "marko";

            NetTcpBinding centralBinding = new NetTcpBinding();
            string centralAddress = "net.tcp://localhost:9999/CentralService";

            centralBinding.Security.Mode = SecurityMode.Transport;
            centralBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 servCert = 
                CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,StoreLocation.LocalMachine, srvCertName);

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(centralAddress),
                new X509CertificateEndpointIdentity(servCert));

            //ClientProxy proxy = new ClientProxy(centralBinding, endpointAddress);

            ClientProxy.proxy = new ClientProxy(centralBinding, endpointAddress);
            
            Console.WriteLine("Uspedno azurirano {0} vrednosti.",ClientProxy.proxy.updateRecords(Local.readFromDB(item.Value))); //Povuce podatke iz lokalnog txt i posalje centralnoj da proveri sinhronizaciju

            #endregion

            Console.ReadLine();
        }
    }
}
