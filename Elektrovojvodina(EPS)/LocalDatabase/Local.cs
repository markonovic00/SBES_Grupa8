using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public List<Data> readFromDB(string region)
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

        public List<Data> getData(string region)
        {
            List<Data> list = readFromDB(region);
            Console.WriteLine("Client request data");
            return list;
        }

        public int Ping()
        {
            Console.WriteLine("Client ping");
            return 1;
        }

        public string writeData(Data data,string region)
        {
            
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
                
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        public List<Data> getDataByCity(string region, string city)
        {
            List<Data> list = readFromDB(region);
            List<Data> filter = new List<Data>();
            if (list != null)
            {
                foreach (Data item in list)
                {
                    if (item.Grad.ToLower().Equals(city))
                        filter.Add(item);
                }
            }

            Console.WriteLine("Client request data");
            return filter;
        }

        public double getAverageByCity(string region,string city, string godina)
        {
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
            average = average / counter;

            return average;
        }

        public double getAverageByRegion(string region, string godina)
        {
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
            average = average / counter;

            return average;
        }

        public Data updateConsumption(string region, string city, double value)
        {
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

            return i;
        }
    }
}
