using ClinicReservation.Helpers;
using Hake.Extension.ValueRecord;
using LocalizationCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public interface INotificationProvider
    {
        string this[ICultureExpression culture] { get; }

        void SetNotification(string notification, string culture);

        void RenewNotification();
    }

    internal sealed class NotificationProvider : INotificationProvider
    {
        private object locker;
        private Dictionary<string, string> notifications;
        public string this[ICultureExpression culture]
        {
            get
            {
                string result = CultureHelper.MatchCulture(notifications, culture, out ICultureExpression matchedCulture);
                if (matchedCulture == null)
                    throw new Exception("language resource not found");
                return result;
            }
        }

        private string filePath;

        public NotificationProvider(string filePath)
        {
            locker = new object();
            this.filePath = filePath;
            this.notifications = new Dictionary<string, string>();
            RenewNotification();
        }

        public void SetNotification(string notification, string culture)
        {
            lock (locker)
            {
                notifications[culture] = notification;
                SetRecord record = new SetRecord();
                foreach (var pair in notifications)
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
        public void RenewNotification()
        {
            lock (locker)
            {
                this.notifications.Clear();

                Stream fileStream = File.OpenRead(filePath);
                SetRecord record = (SetRecord)Hake.Extension.ValueRecord.Json.Converter.ReadJson(fileStream);
                fileStream.Dispose();
                foreach (var pair in record)
                {
                    if (pair.Value is ScalerRecord scaler)
                    {
                        notifications.Add(pair.Key, scaler.ReadAs<string>());
                    }
                }
            }
        }

    }
}
