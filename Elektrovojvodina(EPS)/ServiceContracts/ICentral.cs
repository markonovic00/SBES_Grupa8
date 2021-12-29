using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ServiceContracts
{
    [ServiceContract]
    public interface ICentral
    {
        [OperationContract]
        int Ping();

        [OperationContract]
        int checkMatching(List<Data> localData); // u teoriji bi se mogao raditi neki HASH i onda te HASH vrednosti da se proveravaju samo, ali neka za projekat dovoljno ovo :D

        [OperationContract]
        int writeCentral(Data localData);

        [OperationContract]
        int updateConsumpion(string region, string city, double value);

        [OperationContract]
        int deleteData(Data _data);

        [OperationContract]
        int updateRecords(List<Data> _data);

    }
}
