using ClinicReservation.Helpers;
using Hake.Extension.ValueRecord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public interface IServiceNotAvailableReasonProvider
    {
        string this[CultureExpression culture] { get; }

        void SetReason(string notification, string culture);

        void RenewReason();
    }

    internal sealed class ServiceNotAvailableReasonProvider : IServiceNotAvailableReasonProvider
    {
        private object locker;
        private Dictionary<string, string> reasons;
        public string this[CultureExpression culture]
        {
            get
            {
                string result = CultureHelper.MatchCulture(reasons, culture, out CultureExpression matchedCulture);
                if (matchedCulture == null)
                    throw new Exception("language resource not found");
                return result;
            }
        }

        private string filePath;

        public ServiceNotAvailableReasonProvider(string filePath)
        {
            locker = new object();
            this.filePath = filePath;
            this.reasons = new Dictionary<string, string>();
            RenewReason();
        }

        public void SetReason(string notification, string culture)
        {
            lock (locker)
            {
                reasons[culture] = notification;
                SetRecord record = new SetRecord();
                foreach (var pair in reasons)
                {
                    record.Add(pair.Key, new ScalerRecord(pair.Value));
                }
                string json = Hake.Extension.ValueRecord.Json.Converter.Json(record);
                FileStream stream = File.OpenWrite(filePath);
                stream.SetLength(0);
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                stream.Flush();
                writer.Dispose();
                stream.Dispose();
            }
        }
        public void RenewReason()
        {
            lock (locker)
            {
                this.reasons.Clear();

                Stream fileStream = File.OpenRead(filePath);
                SetRecord record = (SetRecord)Hake.Extension.ValueRecord.Json.Converter.ReadJson(fileStream);
                fileStream.Dispose();
                foreach (var pair in record)
                {
                    if (pair.Value is ScalerRecord scaler)
                    {
                        reasons.Add(pair.Key, scaler.ReadAs<string>());
                    }
                }
            }
        }

    }
}
