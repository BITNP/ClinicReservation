using System.Runtime.Serialization;

namespace ClinicReservation.Services.SMS
{
    [DataContract]
    public class SMSResponse
    {
        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Msg { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public long Sid { get; set; }
    }
}
