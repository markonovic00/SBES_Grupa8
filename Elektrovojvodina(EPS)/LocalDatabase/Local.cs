using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts;


namespace LocalDatabase
{
    public class Local : ILocal
    {
        public static List<Data> localData = new List<Data>();
        public static LocalSettings localSettings = new LocalSettings();
        public static List<Data> readFromDB(string region)
        {
            
            List<Data> retData = new List<Data>();
            if (File.Exists(region + ".txt"))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(region + ".txt"))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                                retData.Add(Data.DataFromString(line)); // Add to list.
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return retData;
        }

        public void updateDB(List<Data> data, string region)
        {
            try
            {
                StreamWriter sw = new StreamWriter(region+".txt"); //Suvoparno upisivanje podataka
                foreach (Data item in data)
                {
                    sw.WriteLine(item.ToString());
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ReadSBES")]
        public byte[] getData(byte[] _region)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            List<Data> list = readFromDB(region);
            string plainTxt = string.Empty;
            foreach (Data item in list)
            {
                plainTxt += item.ToString() + "|";
            }
            plainTxt = plainTxt.Remove(plainTxt.Length - 1);
            byte[] enc = Security.EncryptStringToBytes_Aes(plainTxt, localSettings.Key);
            Console.WriteLine("Client request data");
            return enc;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ReadSBES")]
        public int Ping()
        {
            Console.WriteLine("Client ping");
            return 1;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AdminSBES")]
        public byte[] writeData(byte[] _data, byte[] _region)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            Data data = Data.DataFromString(Security.DecryptStringFromBytes_Aes(_data, localSettings.Key));
            try
            {
                ClientProxy.proxy.Ping(); // Pinguje centralni server radi provere samo 
                //Provera da li postoji vec takav podatak u lokalnoj bazi 
                List<Data> localData = readFromDB(region);
                if (localData == null)
                    localData = new List<Data>();
                bool notexists = true;
                foreach (Data item in localData)
                {
                    if (item.Same(data))
                        notexists = false;

                }
                if(notexists)
                {
                    data.ID = 1;
                    if(localData.Count>0)
                        data.ID = (localData.Last<Data>().ID + 1); // Uveca id novog za 1 od poslednje
                    //Upisujemo novi podatak
                    StreamWriter sw = new StreamWriter(region + ".txt", true); //True je za append
                    sw.WriteLine(data.ToString());
                    sw.Close();
                    Console.WriteLine("Client writing: " + data.ToString());
                    ClientProxy.proxy.writeCentral(data); //radi upisivanje novog u centralnu
                }
                
                return Security.EncryptStringToBytes_Aes("Uspesno upisano",localSettings.Key);
            }
            catch (Exception ex)
            {
                return Security.EncryptStringToBytes_Aes(ex.Message.ToString(),localSettings.Key);
            }

        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ReadSBES")]
        public byte[] getDataByCity(byte[] _region, byte[] _city)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            string city = Security.DecryptStringFromBytes_Aes(_city, localSettings.Key);
            List<Data> list = readFromDB(region);
            string messRet = string.Empty;
            if (list != null)
            {
                foreach (Data item in list)
                {
                    if (item.Grad.ToLower().Equals(city))
                        messRet += item.ToString() + "|";
                }
            }
            messRet = messRet.Remove(messRet.Length - 1);
            byte[] enc = Security.EncryptStringToBytes_Aes(messRet, localSettings.Key);
            Console.WriteLine("Client request data");
            return enc;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "CalculateSBES")]
        public byte[] getAverageByCity(byte[] _region, byte[] _city, byte[] _year)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            string city = Security.DecryptStringFromBytes_Aes(_city, localSettings.Key);
            string godina = Security.DecryptStringFromBytes_Aes(_year, localSettings.Key);
            List<Data> data = readFromDB(region);
            if (data == null)
                data = new List<Data>();

            double average = 0;
            int counter = 0;

            foreach (Data item in data)
            {
                if (item.Grad.ToLower().Equals(city) && item.Godina.Equals(godina))
                {
                    average = item.MesecnaPotrosnja.Sum();
                    counter = item.MesecnaPotrosnja.Count;
                }
            }
            try
            {
                average = average / counter;
            }
            catch(Exception ex)
            {
                average = 0;
            }

            byte[] encMess = Security.EncryptStringToBytes_Aes(average.ToString(), localSettings.Key);

            return encMess;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "CalculateSBES")]
        public byte[] getAverageByRegion(byte[] _region, byte[] _year)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            string godina = Security.DecryptStringFromBytes_Aes(_year, localSettings.Key);
            List<Data> data = readFromDB(region);
            if (data == null)
                data = new List<Data>();

            double average = 0;
            int counter = 0;

            foreach (Data item in data)
            {
                if (item.Region.ToLower().Equals(region) && item.Godina.Equals(godina))
                {
                    average += item.MesecnaPotrosnja.Sum();
                    counter += item.MesecnaPotrosnja.Count;
                }
            }
            try
            {
                average = average / counter;
            }
            catch (Exception ex)
            {
                average = 0;
            }

            byte[] encMess = Security.EncryptStringToBytes_Aes(average.ToString(), localSettings.Key);

            return encMess;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ModifySBES")]
        public byte[] updateConsumption(byte[] _region, byte[] _city, byte[] _value)
        {
            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            string city = Security.DecryptStringFromBytes_Aes(_city, localSettings.Key);
            double value = Convert.ToDouble(Security.DecryptStringFromBytes_Aes(_value, localSettings.Key));

            Data i = new Data() {ID=0,Grad="",Region="",Godina="",MesecnaPotrosnja=new List<double>() { 0 } };
            int currentMounth = DateTime.Now.Month;

            List<Data> data = readFromDB(region);
            if (data == null)
                data = new List<Data>();

            bool need = false;
            foreach (Data item in data)
            {
                if (item.Grad.ToLower().Equals(city.ToLower()) && item.Godina.Equals(DateTime.Now.Year.ToString()))
                {  
                    item.MesecnaPotrosnja[currentMounth - 1] = value;
                    i = item;
                    need = true;
                }
            }

            if (need)
            {
                updateDB(data, region); // Need to update it in the central DB
                ClientProxy.proxy.updateConsumpion(region, city, value); // Update Central DB
            }

            return Security.EncryptStringToBytes_Aes(i.ToString(),localSettings.Key);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "AdminSBES")]
        public byte[] deleteData(byte[] _data, byte[] _region)
        {
            int i = -1;

            string region = Security.DecryptStringFromBytes_Aes(_region, localSettings.Key);
            Data data = Data.DataFromString(Security.DecryptStringFromBytes_Aes(_data, localSettings.Key));

            List<Data> Locdata = readFromDB(region);
            if (Locdata == null)
                Locdata = new List<Data>();

            i = data.ID;
            foreach (Data item in Locdata)
            {
                if (item.Same(data))
                {
                    Locdata.Remove(item);
                    break;
                }
            }
            // Regulisemo indekse ispocetka
            for(int j = 0; j < Locdata.Count; j++)
            {
                Locdata[j].ID = j + 1;
            }
            
            updateDB(Locdata, region); // Need to update it in the central DB
            ClientProxy.proxy.deleteData(data); //Brisanje u centralnoj bazi
            

            return Security.EncryptStringToBytes_Aes(i.ToString(),localSettings.Key);
        }
    }
}
