using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public int checkMatching(List<Data> localData)
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

            bool notexists = true;
            foreach (Data item in centralData)
            {
                if (item.Same(localData))
                    notexists = false;
            } // dodamo nov element

            if (notexists)
            {
                localData.ID = 1;
                if(centralData.Count>0)
                    localData.ID = centralData.Last<Data>().ID+1;
            }

            //upisemo ih u bazu ponovo
            StreamWriter sw = new StreamWriter("Central.txt", true); //True je za append
            sw.WriteLine(localData.ToString());
            sw.Close();

            return updatedRecords;
        }
    }
}
