using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public class ServiceConfig
    {
        public string ConnectionString { get; set; }
        public string SessionName { get; set; }
        public string SecurityKey { get; set; }
        public string SMSUrl { get; set; }
        public string SMSApiKey { get; set; }
        public string RegisterationTicket { get; set; }
        public string NotificationPath { get; set; }
        public string ServiceReasonPath { get; set; }



        public static string ReadTicket()
        {
            try
            {
                FileStream stream = File.OpenRead("ticket.txt");
                StreamReader reader = new StreamReader(stream);
                string ticket = reader.ReadToEnd();
                reader.Dispose();
                stream.Dispose();
                return ticket;
            }
            catch
            {
                return null;
            }
        }
    }
}
