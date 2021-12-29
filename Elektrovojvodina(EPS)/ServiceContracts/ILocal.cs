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
        List<Data> getData(string region);

        [OperationContract]
        List<Data> getDataByCity(string region, string city);

        [OperationContract]
        double getAverageByCity(string region, string city, string godina);

        [OperationContract]
        double getAverageByRegion(string region, string godina);

        [OperationContract]
        Data updateConsumption(string region, string city, double value);

        [OperationContract]
        string writeData(Data data, string region);

        [OperationContract]
        int deleteData(Data data, string region);
    }
}
