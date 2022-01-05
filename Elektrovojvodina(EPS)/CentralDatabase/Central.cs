using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Manager;

namespace CentralDatabase
{
    class Central : ICentral
    {
        public List<Data> readFromDB()
        {
            List<Data> retData = new List<Data>();
            try
            {
                using (StreamReader reader = new StreamReader("Central.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(!string.IsNullOrWhiteSpace(line))
                            retData.Add(Data.DataFromString(line)); // Add to list.
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return retData;
        }

        public void writeToDB(List<Data> data)
        {
            try
            {
                StreamWriter sw = new StreamWriter("Central.txt"); //Suvoparno upisivanje podataka
                foreach (Data item in data)
                {
                    sw.WriteLine(item.ToString());
                }
                sw.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int checkMatching(List<Data> localData) //Nepotrebna funckija radi vezbe tu
        {
            int notMatching = 0;

            List<Data> centralData = readFromDB();
            if(centralData.Count!=0)
            {
                List<Data> centralFiltered = new List<Data>();
                foreach (Data item in centralData)
                {
                    if (item.Region.Equals(localData[0].Region))
                    {
                        centralFiltered.Add(item);
                    }
                }

                foreach (Data item in localData)
                {
                    foreach (Data cenItem in centralFiltered)
                    {
                        if (!cenItem.Same(item)) //ako se vrednosti nisu jednake znaci da se povecava broj 'nejednakih podataka' potrebno azuriranje
                            notMatching++;
                    }
                }
            }

            return notMatching;
        }

        public int Ping()
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Tip autentifikacije: " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Ime LokalneBaze koja je pingovala: "+ windowsIdentity.Name);
            Console.WriteLine("Jedinstveni identifikator: "+ windowsIdentity.User);

            return 1;
        }

        public int writeCentral(Data localData)
        {
            int updatedRecords = 0;
            List<Data> centralData = readFromDB(); //Pokupimo sve stare podatke

            if (centralData.Count == 0 || centralData == null)
                centralData = new List<Data>();

            bool notexists = true; //Proverimo da li postoji u bazi vec
            foreach (Data item in centralData)
            {
                if (item.Same(localData))
                    notexists = false;
            } 
            //Dodamo novi ako ne postoji, postavimo mu novi indeks
            if (notexists)
            {
                localData.ID = 1;
                if(centralData.Count>0)
                    localData.ID = centralData.Last<Data>().ID+1;
            }

            //upisemo novi u bazu
            StreamWriter sw = new StreamWriter("Central.txt", true); //True je za append
            sw.WriteLine(localData.ToString());
            sw.Close();

            try
            {
                Audit.WriteCentral(localData.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Local database writing: "+ localData.ToString());

            return updatedRecords;
        }

        public int updateConsumpion(string region, string city, double value)
        {
            int retId = 0;
            int currentMounth = DateTime.Now.Month;
            List<Data> data = readFromDB(); //Pokupimo sve stare podatke

            if (data.Count == 0 || data == null)
                data = new List<Data>();
            Data dummy = new Data();
            foreach (Data item in data)
            {
                if (item.Grad.ToLower().Equals(city.ToLower()) && item.Godina.Equals(DateTime.Now.Year.ToString()) && item.Region.Equals(region))
                {
                    item.MesecnaPotrosnja[currentMounth - 1] = value;
                    dummy = item;
                    retId = item.ID;
                }
            }

            writeToDB(data);

            Console.WriteLine("Local database updated: "+dummy.ToString());

            try
            {
                Audit.UpdateConsumptionCentral(dummy.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retId;
        }

        public int deleteData(Data _data)
        {
            int retId = -1;
            List<Data> data = readFromDB(); //Pokupimo sve stare podatke

            if (data.Count == 0 || data == null)
                data = new List<Data>();

            retId = _data.ID;
            foreach (Data item in data)
            {
                if (item.Same(_data))
                {
                    try
                    {
                        Audit.DeleteCentral(item.ToString());

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Console.WriteLine("Local database deleted: "+item.ToString());
                    data.Remove(item);

                    break;
                }
            }

            //Regulisemo indekse
            for(int i=0; i < data.Count; i++)
            {
                data[i].ID = i + 1;
            }

            writeToDB(data);

            return retId;
        }

        public int updateRecords(List<Data> _data)
        {
            Console.WriteLine("Local database updating not matching records.");
            int updatedRecords = 0;
            List<Data> centralData = readFromDB(); //Pokupimo sve stare podatke

            if (centralData.Count == 0 || centralData == null)
                centralData = new List<Data>();

            bool notExists = true;
            foreach (Data localItem in _data)
            {
                notExists = true;
                foreach (Data centralItem in centralData)
                {
                    if (localItem.Same(centralItem))
                        notExists = false;
                }
                if (notExists)
                {
                    localItem.ID = centralData.Count + 1;
                    centralData.Add(localItem);
                    updatedRecords++;
                }
            }

            writeToDB(centralData);

            try
            {
                Audit.UpdateAllCentral(updatedRecords.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return updatedRecords;
        }
    }
}
