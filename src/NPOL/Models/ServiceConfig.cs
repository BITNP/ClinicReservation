using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NPOL.Models
{
    public static class ServiceConfig
    {
        public static string ConnectionString { get; set; }
        public static string SessionName { get; set; }
        public static string SecurityKey { get; set; }

        public static string RegisterationTicket { get; set; }

        public static void ReadTicket()
        {
            try
            {
                FileStream stream = File.OpenRead("ticket.txt");
                StreamReader reader = new StreamReader(stream);
                string ticket = reader.ReadToEnd();
                RegisterationTicket = ticket;
                reader.Dispose();
                stream.Dispose();
            }
            catch
            {
                RegisterationTicket = null;
            }
        }
    }
}
