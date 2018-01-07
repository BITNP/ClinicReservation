using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public interface IServiceState
    {
        bool AllowCreate { get; }

        bool SetState(bool allowCreate);

        bool RenewState();
    }

    internal sealed class ServiceState : IServiceState
    {
        private object locker;
        private bool state;
        public bool AllowCreate => state;

        private string filePath;

        public ServiceState(string filePath)
        {
            locker = new object();
            this.filePath = filePath;
            RenewState();
        }

        public bool SetState(bool allowCreate)
        {
            lock (locker)
            {
                state = allowCreate;
                string content = state.ToString();
                FileStream stream = File.OpenWrite(filePath);
                stream.SetLength(0);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(content);
                writer.Flush();
                stream.Flush();
                writer.Dispose();
                stream.Dispose();
                return state;
            }
        }
        public bool RenewState()
        {
            lock (locker)
            {
                Stream fileStream = File.OpenRead(filePath);
                StreamReader reader = new StreamReader(fileStream);
                string content = reader.ReadToEnd();
                bool allowCreate = bool.Parse(content);
                reader.Dispose();
                fileStream.Dispose();
                state = allowCreate;
                return state;
            }
        }
    }
}
