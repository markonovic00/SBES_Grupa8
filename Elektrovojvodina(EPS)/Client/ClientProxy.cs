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
