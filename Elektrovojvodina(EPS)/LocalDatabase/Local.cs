using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts;


namespace LocalDatabase
{
    public class Local : ILocal
    {

        public List<Data> getData(string region)
        {
            List<Data> list = new List<Data>();
            using (StreamReader reader = new StreamReader(region + ".txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(Data.DataFromString(line)); // Add to list.
                }
            }
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
                StreamWriter sw = new StreamWriter(region + ".txt", true); //True je za append
                sw.WriteLine(data.ToString());
                sw.Close();
                Console.WriteLine("Client writing: "+data.ToString());
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
    }
}
