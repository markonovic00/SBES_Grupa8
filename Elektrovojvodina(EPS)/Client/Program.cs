using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
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

            #region AES

            var key = localSettings.Key; // Postavljanje kljuca koji se nalazi u lokalnim podesavanjima

            // Potrebno izmaniti sve fukncije da rade sa nizom bajtova

            byte[] enc = Security.EncryptStringToBytes_Aes("PorukaTest", key);

            string roundtrip = Security.DecryptStringFromBytes_Aes(enc, key);

            Console.WriteLine("Original: {0}","PorukaTest");
            Console.WriteLine("Encrypted (b64-encode): {0}",Convert.ToBase64String(enc));
            Console.WriteLine("Round Trip: {0}",roundtrip);

            #endregion AES

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
                    Console.WriteLine("6. Delete record");
                    Console.WriteLine("7. Add Data");
                    Console.WriteLine("0. Exit");
                    code = Console.ReadLine();
                    switch (code)
                    {
                        case "1":
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.getData(Security.EncryptStringToBytes_Aes(item.Value, key));
                                string plainMess = "";
                                if (encMess.Length > 16)
                                    plainMess = Security.DecryptStringFromBytes_Aes(encMess, key);
                                string[] data = plainMess.Split('|');
                                foreach (var dt in data)
                                {
                                    Console.WriteLine(dt);
                                }
                                
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "2":
                            Console.Write("\t Unesite grad:");
                            string grad = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.getDataByCity(Security.EncryptStringToBytes_Aes(item.Value, key),Security.EncryptStringToBytes_Aes(grad.ToLower().Trim(),key));
                                string plainMess = "";
                                if (encMess.Length>16)
                                    plainMess = Security.DecryptStringFromBytes_Aes(encMess, key);
                                string[] data = plainMess.Split('|');
                                foreach (var dt in data)
                                {
                                    Console.WriteLine(dt);
                                }

                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "3":
                            Console.Write("\t Unesite grad:");
                            string avg = Console.ReadLine();
                            Console.Write("\t Unesite godinu:");
                            string godinac = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.getAverageByCity(
                                    Security.EncryptStringToBytes_Aes(item.Value,key)
                                    ,Security.EncryptStringToBytes_Aes(avg.ToLower().Trim(),key)
                                    ,Security.EncryptStringToBytes_Aes(godinac.ToLower().Trim(),key));
                                string data = "";
                                if (encMess.Length >16)
                                    data = Security.DecryptStringFromBytes_Aes(encMess, key);
                                Console.WriteLine("Potrosnja godine: {0} \n Grad: {1} \n Potrosnja: {2}",godinac,avg,data);
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "4":
                            Console.Write("\t Unesite region:");
                            string avr = Console.ReadLine();
                            Console.Write("\t Unesite godinu:");
                            string godinar = Console.ReadLine();
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.getAverageByRegion(
                                    Security.EncryptStringToBytes_Aes(avr.ToLower().Trim(),key)
                                    ,Security.EncryptStringToBytes_Aes(godinar.ToLower().Trim(),key));
                                string data = "";
                                if (encMess.Length > 16)
                                    data = Security.DecryptStringFromBytes_Aes(encMess, key);
                                Console.WriteLine("Potrosnja godine: {0} \n Grad: {1} \n Potrosnja: {2}", godinar, avr, data);
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "5":
                            Console.Write("\t Unesite grad:");
                            string gradu = Console.ReadLine();
                            Console.Write("\t Unesite potrosnju:");
                            double potrosnja = Convert.ToDouble(Console.ReadLine());
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.updateConsumption(
                                    Security.EncryptStringToBytes_Aes(item.Value,key),
                                    Security.EncryptStringToBytes_Aes(gradu,key),
                                    Security.EncryptStringToBytes_Aes(potrosnja.ToString(),key));
                                string data = "";
                                if (encMess.Length > 16)
                                    data = Security.DecryptStringFromBytes_Aes(encMess, key);
                                Console.WriteLine("Azurirana vrednost: {0}", data);
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "6":
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                byte[] encMess = proxy.getData(Security.EncryptStringToBytes_Aes(item.Value, key));
                                string plainMess = "";
                                if (encMess.Length>16)
                                     plainMess = Security.DecryptStringFromBytes_Aes(encMess, key);
                                string[] data = plainMess.Split('|');
                                foreach (var dt in data)
                                {
                                    Console.WriteLine(dt);
                                }
                                Console.Write("\t Izaberite: ");
                                int index = Convert.ToInt32(Console.ReadLine().Trim());
                                string delData = data[index - 1];
                                byte[] encInd = proxy.deleteData(Security.EncryptStringToBytes_Aes(delData,key),
                                    Security.EncryptStringToBytes_Aes(item.Value,key));
                                string redInd = "";
                                if (encInd.Length>16)
                                    redInd = Security.DecryptStringFromBytes_Aes(encInd, key);
                                Console.WriteLine("Uspesno obrisano na indeksu: " + redInd);
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
                            }
                            break;
                        case "7":
                            if (proxy.Ping() == 1) //Prilikom svakog poziva metode izvrsiti ping
                            {
                                Data dt = new Data
                                {
                                    ID = 0,
                                    Region = item.Value,
                                    Grad = "",
                                    Godina = "",
                                    MesecnaPotrosnja = new List<double>()
                                };
                                Console.Write("\tUnesite grad: ");
                                dt.Grad = Console.ReadLine().Trim();
                                Console.Write("\tUnesite godinu: ");
                                dt.Godina = Console.ReadLine().Trim();
                                Console.WriteLine("\tUnesite mesecnje potrosnje: ");
                                for (int i = 0; i < 12; i++)
                                {
                                    Console.Write("\t  {0}. Mesec: ",i+1);
                                    dt.MesecnaPotrosnja.Add(Convert.ToDouble(Console.ReadLine().Trim()));
                                }
                                byte[] encMess = proxy.writeData(Security.EncryptStringToBytes_Aes(dt.ToString(),key), 
                                    Security.EncryptStringToBytes_Aes(item.Value,key));
                                string retMess = "";
                                if (encMess.Length>16)
                                    retMess = Security.DecryptStringFromBytes_Aes(encMess, key);
                                Console.WriteLine(retMess);
                            }
                            else
                            {
                                //Console.WriteLine("Neuspesna konekcija sa servisom...");
                                //Console.ReadKey();
                                //Environment.Exit(0);
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
