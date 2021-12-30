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

        public byte[] deleteData(byte[] data, byte[] region)
        {
            try
            {
                return factory.deleteData(data, region);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] { 0 };
            }
        }

        public byte[] getAverageByCity(byte[] _region, byte[] _city, byte[] _year)
        {
            try
            {
                return factory.getAverageByCity(_region,_city, _year);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] {0};
            }
        }

        public byte[] getAverageByRegion(byte[] region, byte[] godina)
        {
            try
            {
                return factory.getAverageByRegion(region, godina);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] { 0 };
            }
        }

        public byte[] getData(byte[] _region)
        {
            try
            {
                return factory.getData(_region);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] {0};
            }
        }

        public byte[] getDataByCity(byte[] _region, byte[] _city)
        {
            try
            {
                return factory.getDataByCity(_region,_city);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] { 0 };
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

        public byte[] updateConsumption(byte[] region, byte[] city, byte[] value)
        {
            try
            {
                return factory.updateConsumption(region, city, value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return new byte[] { 0 };
            }
        }

        public byte[] writeData(byte[] data,byte[] region)
        {
            try
            {
                return factory.writeData(data, region);
            }
            catch (Exception e)
            {
                
                return new byte[] { 0 };
            }
        }
    }
}
