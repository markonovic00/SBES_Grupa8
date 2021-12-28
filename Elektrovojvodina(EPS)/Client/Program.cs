using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
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

                if (IsBusy(item.Key))
                    break;
            }

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:" + item.Key + "/" + item.Value;

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),
                EndpointIdentity.CreateUpnIdentity("localServer"));

            using (ClientProxy proxy = new ClientProxy(binding, endpointAddress))
            {
                string code = "";
                while (true)
                {
                    Console.WriteLine("1. Get Data");
                    Console.WriteLine("2. Get Data by city");
                    Console.WriteLine("3. Get Year Average by city");
                    Console.WriteLine("4. Get Year Average by region");
                    Console.WriteLine("5. Update consumption this mounth");
                    Console.WriteLine("6. Add Data");
                    Console.WriteLine("0. Exit");
                    code = Console.ReadLine();
                    switch (code)
                    {
                        case "1":
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                List<Data> data = proxy.getData(item.Value);
                                foreach (Data dt in data)
                                {
                                    Console.WriteLine(dt.ToString());
                                }
                                
                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                        case "2":
                            Console.Write("\t Unesite grad:");
                            string grad = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                List<Data> data = proxy.getDataByCity(item.Value,grad.ToLower().Trim());
                                foreach (Data dt in data)
                                {
                                    Console.WriteLine(dt.ToString());
                                }

                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                        case "3":
                            Console.Write("\t Unesite grad:");
                            string avg = Console.ReadLine();
                            Console.Write("\t Unesite godinu:");
                            string godinac = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                double data = proxy.getAverageByCity(item.Value,avg.ToLower().Trim(),godinac.ToLower().Trim());
                                Console.WriteLine("Potrosnja godine: {0} \n Grad: {1} \n Potrosnja: {2}",godinac,avg,data);
                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                        case "4":
                            Console.Write("\t Unesite region:");
                            string avr = Console.ReadLine();
                            Console.Write("\t Unesite godinu:");
                            string godinar = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                double data = proxy.getAverageByRegion(avr.ToLower().Trim(), godinar.ToLower().Trim());
                                Console.WriteLine("Potrosnja godine: {0} \n Grad: {1} \n Potrosnja: {2}", godinar, avr, data);
                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                        case "5":
                            Console.Write("\t Unesite grad:");
                            string gradu = Console.ReadLine();
                            Console.Write("\t Unesite potrosnju:");
                            double potrosnja = Convert.ToDouble(Console.ReadLine());
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                Data data = proxy.updateConsumption(item.Value, gradu, potrosnja);
                                Console.WriteLine("Azurirana vrednost: {0}",data.ToString());
                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                        case "6":
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                Data dt = new Data
                                {
                                    ID = 1,
                                    Region = item.Value,
                                    Grad = "GradNeki",
                                    Godina = DateTime.Now.Year.ToString(),
                                    MesecnaPotrosnja = new List<double> { 1, 2, 1, 3, 4 }
                                };
                                Console.WriteLine(proxy.writeData(dt,item.Value));
                            }
                            else
                            {
                                Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                Environment.Exit(0);
                            }
                            break;
                    }

                    if (code == "0")
                        break;
                    
                }
            }
            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }
    }
}
