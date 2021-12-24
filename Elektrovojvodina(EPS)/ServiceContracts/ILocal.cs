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
        string writeData(Data data, string region);
    }
}
