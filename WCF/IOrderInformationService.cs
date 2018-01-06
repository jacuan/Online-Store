using DigitalXData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF
{
    [ServiceContract]
    public interface IOrderInformationService
    {
        [OperationContract]
        [FaultContract(typeof(RequestFailed))]
        List<OrderedItem> GetOrderList(int id);

        [OperationContract]
        [FaultContract(typeof(RequestFailed))]
        List<ShoppingCartInformation> GetShoppingCartInformation(int id);
    }

    [DataContract]
    public class RequestFailed
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int ErrorCode { get; set; }
    }
}
