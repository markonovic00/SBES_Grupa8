using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ServiceContracts
{
    [DataContract]
    public class Data
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Region { get; set; }
        [DataMember]
        public string Grad { get; set; }
        [DataMember]
        public string Godina { get; set; }
        [DataMember]
        public List<Double> MesecnaPotrosnja { get; set; }

        public Data()
        {

        }

        public static Data DataFromString(string dataString)
        {
            Data newDt = new Data();
            string[] listDt = dataString.Split(';');
            newDt.ID = Convert.ToInt32(listDt[0]);
            newDt.Region = listDt[1];
            newDt.Grad = listDt[2];
            newDt.Godina = listDt[3];
            listDt[4] = listDt[4].Remove(0,1);
            listDt[4] = listDt[4].Remove(listDt[4].Length - 1);
            string[] meseci = listDt[4].Split(',');
            newDt.MesecnaPotrosnja = new List<double>();
            foreach (var item in meseci)
            {
                newDt.MesecnaPotrosnja.Add(Convert.ToDouble((item)));
            }
            return newDt;
        }

        public override string ToString()
        {
            string mp = "[";
            foreach (var item in MesecnaPotrosnja)
            {
                mp += item + ",";
            }
            mp=mp.Remove(mp.Length - 1);
            mp += "]";
            return ID + ";" + Region + ";" + Grad + ";" + Godina + ";" + mp;
        }

        public bool Same(Data newData)
        {
            bool ret = false;
            if (Region == newData.Region && Grad == newData.Grad && Godina == newData.Godina) //Dodati proveru za mesecnu potrosnju
                ret = true;
            else
                ret = false;

            return ret;
        }
    }
}
