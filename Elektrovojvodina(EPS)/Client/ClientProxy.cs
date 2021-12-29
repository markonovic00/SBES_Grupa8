using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<ILocal>, ILocal, IDisposable
    {
        ILocal factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
            //Credentials.Windows.AllowNtlm = false;
        }

        public int deleteData(Data data, string region)
        {
            try
            {
                return factory.deleteData(data, region);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public double getAverageByCity(string region, string city, string godina)
        {
            try
            {
                return factory.getAverageByCity(region,city, godina);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public double getAverageByRegion(string region, string godina)
        {
            try
            {
                return factory.getAverageByRegion(region, godina);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public List<Data> getData(string region)
        {
            try
            {
                return factory.getData(region);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new List<Data>() { new Data { ID = 0, Region = "", Grad = "", Godina = "", MesecnaPotrosnja = new List<double>() { 0, } } };
            }
        }

        public List<Data> getDataByCity(string region, string city)
        {
            try
            {
                return factory.getDataByCity(region,city);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new List<Data>() { new Data { ID = 0, Region = "", Grad = "", Godina = "", MesecnaPotrosnja = new List<double>() { 0, } } };
            }
        }

        public int Ping()
        {
            try
            {
                return factory.Ping();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public Data updateConsumption(string region, string city, double value)
        {
            try
            {
                return factory.updateConsumption(region, city, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new Data { ID = 0, Region = "", Grad = "", Godina = "", MesecnaPotrosnja = new List<double>() { 0, } };
            }
        }

        public string writeData(Data data,string region)
        {
            try
            {
                return factory.writeData(data, region);
            }
            catch (Exception e)
            {
                
                return  e.Message ;
            }
        }
    }
}
