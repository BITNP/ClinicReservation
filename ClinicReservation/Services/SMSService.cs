using Microsoft.Extensions.Logging;
using ClinicReservation.Models.Reservation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ClinicReservation.Services
{
    public sealed class SMSService
    {
        public static readonly EventId SUCCESS_EVENT = new EventId(0, "sms_success");
        public static readonly EventId FAILED_EVENT = new EventId(1, "sms_failed");

        private string smsuri;
        private ILogger logger;
        public SMSService(ServiceConfig serviceConfig, ILogger smsLogger)
        {
            if (serviceConfig.SMSApi == null)
                throw new Exception("SMS Api not configured");
            smsuri = serviceConfig.SMSApi;
            this.logger = smsLogger;
        }

        // 用于向申请者发送短信
        // 通知创建申请成功
        public Task SendCreationSuccessAsync(ReservationDetail reservation)
        {
            string phone = reservation.PosterPhone;
            throw new NotImplementedException();
        }

        // 用于向诊所人员发送短信
        // 通知有新的申请
        public Task SendReservationCreatedAsync(ReservationDetail reservation)
        {
            throw new NotImplementedException();
        }

        // 发送短信底层
        private Task SendSMSAsync(string phone, string message)
        {
            return Task.Run(async () =>
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(smsuri);
                // TODO: 填写发送短信逻辑

                try
                {
                    WebResponse response = await request.GetResponseAsync();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseReader = new StreamReader(responseStream);
                    string responseContent = await responseReader.ReadToEndAsync();
                    // TODO: 填写完成逻辑

                    logger.LogInformation(SUCCESS_EVENT, $"to:{phone}");
                }
                catch (Exception ex)
                {
                    logger.LogError(FAILED_EVENT, ex, $"to:{phone}");
                }
            });
        }
    }
}
