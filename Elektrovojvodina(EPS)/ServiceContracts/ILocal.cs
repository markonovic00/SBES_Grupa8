using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ServiceContracts
{
    [ServiceContract]
    public interface ILocal
    {
        [OperationContract]
        int Ping();

        [OperationContract]
        byte[] getData(byte[] _region);

        [OperationContract]
        byte[] getDataByCity(byte[] _region, byte[] _city);

        [OperationContract]
        byte[] getAverageByCity(byte[] _region, byte[] _city, byte[] _year);

        [OperationContract]
        byte[] getAverageByRegion(byte[] _region, byte[] _year);

        [OperationContract]
        byte[] updateConsumption(byte[] _region, byte[] _city, byte[] _value);

        [OperationContract]
        byte[] writeData(byte[] _data, byte[] _region);

        [OperationContract]
        byte[] deleteData(byte[] _data, byte[] _region);
    }
}
