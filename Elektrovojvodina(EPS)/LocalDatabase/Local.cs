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
    }
}
