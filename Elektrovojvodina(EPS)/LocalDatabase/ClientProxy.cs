using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LocalDatabase
{
    class ClientProxy : ChannelFactory<ICentral>, ICentral, IDisposable
    {
        ICentral factory;
        public static ClientProxy proxy { get; set; }
        

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
            //Credentials.Windows.AllowNtlm = false;
        }

        public int Ping()
        {
            try
            {
                return factory.Ping();
            }
            catch (Exception e)
            {                
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int checkMatching(List<Data> localData)
        {
            try
            {
                return factory.checkMatching(localData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int writeCentral(Data localData)
        {
            try
            {
                return factory.writeCentral(localData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }

        public int updateConsumpion(string region, string city, double value)
        {
            try
            {
                return factory.updateConsumpion(region, city, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return -1;
            }
        }
    }
}
