using Microsoft.Extensions.Logging;
using ClinicReservation.Models.Reservation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ClinicReservation.Services
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

    public sealed class SMSService
    {
        public static readonly EventId SUCCESS_EVENT = new EventId(0, "sms_success");
        public static readonly EventId FAILED_EVENT = new EventId(1, "sms_failed");

        private string smsuri;
        private ILogger logger;
        public SMSService(ServiceConfig serviceConfig, ILoggerFactory loggerFactory)
        {
            if (serviceConfig.SMSApi == null)
                throw new Exception("SMS Api not configured");
            smsuri = serviceConfig.SMSApi;
            this.logger = loggerFactory.CreateLogger<SMSService>();
        }

        // 用于向申请者发送短信
        // 通知创建申请成功
        public Task SendCreationSuccessAsync(ReservationDetail reservation, CultureExpression culture)
        {
            string phone = reservation.PosterPhone;
            return Task.CompletedTask;
        }

        // 用于向申请者发送短信
        // 通知已受理
        public Task SendAnsweredAsync(ReservationDetail reservation)
        {
            string phone = reservation.PosterPhone;
            return Task.CompletedTask;
        }

        // 用于向诊所人员发送短信
        // 通知有新的申请
        public Task SendReservationCreatedAsync(ReservationDetail reservation)
        {
            return Task.CompletedTask;
        }

        // 用于向诊所人员发送短信
        // 通知申请被用户主动关闭
        public Task SendReservationClosedAsync(ReservationDetail reservation)
        {
            string phone = reservation.PosterPhone;
            return Task.CompletedTask;
        }

        // 用于向诊所人员发送短信
        // 通知申请被用户主动取消
        public Task SendReservationCancelledAsync(ReservationDetail reservation)
        {
            string phone = reservation.PosterPhone;
            return Task.CompletedTask;
        }

        // 用于向诊所人员发送短信
        // 当预约更改时通知受理该问题的人员
        public Task SendReservationUpdatedAsync(ReservationDetail reservation)
        {
            string phone = reservation.PosterPhone;
            return Task.CompletedTask;
        }


        // 发送短信底层
        private Task SendSMSAsync(string phone, string message)
        {
            return Task.Run(async () =>
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(smsuri);
                // TODO: 填写发送短信逻辑
                string apikey = " ";
                string data_send_sms = "apikey=" + apikey + "&mobile=" + phone + "&text=" + message;
                byte[] postdatabyte = Encoding.UTF8.GetBytes(data_send_sms);
                request.ContentLength = postdatabyte.Length;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postdatabyte, 0, postdatabyte.Length);
                requestStream.Close();

                try
                {
                    WebResponse response = await request.GetResponseAsync();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseReader = new StreamReader(responseStream);
                    string responseContent = await responseReader.ReadToEndAsync();
                    // TODO: 填写完成逻辑

                    var ms = new MemoryStream(Encoding.Unicode.GetBytes(responseContent));
                    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(SMSResponse));
                    SMSResponse sMSResponse = (SMSResponse)deseralizer.ReadObject(ms); //反序列化ReadObject
                    if(sMSResponse.Code != 0)
                    {
                        throw new Exception(sMSResponse.Msg);
                    }

                    logger.LogInformation(SUCCESS_EVENT, $"to:{phone},{sMSResponse.Msg}");
                }
                catch (Exception ex)
                {
                    logger.LogError(FAILED_EVENT, ex, $"to:{phone},{ex}");
                }
            });
        }
    }
}
