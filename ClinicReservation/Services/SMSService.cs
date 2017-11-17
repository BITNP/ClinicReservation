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

        private string smsurl;
        private string smsapikey;
        private ILogger logger;

        public string SMSCreationSuccess;
        public string SMSAnswered;
        //public string SMSReservationCreated;
        public string SMSReservationClosed;
        public string SMSReservationCancelled;
        public string SMSReservationUpdated;

        public SMSService(ServiceConfig serviceConfig, ILoggerFactory loggerFactory)
        {
            if (serviceConfig.SMSUrl == null)
                throw new Exception("SMS URL not configured");
            smsurl = serviceConfig.SMSUrl;
            if (serviceConfig.SMSApiKey == null)
                throw new Exception("SMS API Key not configured");
            smsapikey = serviceConfig.SMSApiKey;
            this.logger = loggerFactory.CreateLogger<SMSService>();

            SMSCreationSuccess = serviceConfig.SMSCreationSuccess;
            SMSAnswered = serviceConfig.SMSAnswered;
            //SMSReservationCreated = serviceConfig.SMSReservationCreated;
            SMSReservationClosed = serviceConfig.SMSReservationClosed;
            SMSReservationCancelled = serviceConfig.SMSReservationCancelled;
            SMSReservationUpdated = serviceConfig.SMSReservationUpdated;
        }

        // 用于向申请者发送短信
        // 通知创建申请成功
        public Task SendCreationSuccessAsync(ReservationDetail reservation, CultureExpression culture)
        {
            if (reservation.PosterPhone != null && reservation.PosterName != null && SMSCreationSuccess != null)
            {
                string phone = reservation.PosterPhone;
                string name = reservation.PosterName;
                int ID = reservation.Id;
                string message = SMSCreationSuccess.Replace("#name#", name).Replace("#id#", ID + "");
                return SendSMSAsync(phone, message);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        // 用于向申请者发送短信
        // 通知已受理
        public Task SendAnsweredAsync(ReservationDetail reservation)
        {
            if (reservation.PosterPhone != null && reservation.PosterName != null && SMSAnswered != null)
            {

                string phone = reservation.PosterPhone;
                string name = reservation.PosterName;
                string workerName = reservation.DutyMember.Name;
                string message = SMSAnswered.Replace("#name#", name).Replace("#workerName#", workerName);
                return SendSMSAsync(phone, message);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        // 用于向诊所人员发送短信
        // 通知有新的申请
        public Task SendReservationCreatedAsync(ReservationDetail reservation)
        {
            //if (reservation.PosterPhone != null && reservation.PosterName != null && SMSReservationCreated != null)
            //{

            //    string phone = reservation.DutyMember.Contact;
            //    string name = reservation.PosterName;
            //    string workerName = reservation.DutyMember.Name;
            //    string message = SMSAnswered.Replace("#name#", name).Replace("#workerName#", workerName);
            //    //string message = "【北理网协】" + workerName + "您好，您有来自" + name + "的新预约，请及时登录系统受理，谢谢。";
            //    return SendSMSAsync(phone, message);
            //}
            //else
            //{
            //    return Task.CompletedTask;
            //}

            return Task.CompletedTask;
        }

        // 用于向诊所人员发送短信
        // 通知申请被用户主动关闭
        public Task SendReservationClosedAsync(ReservationDetail reservation)
        {
            if (reservation.DutyMember != null && SMSReservationClosed != null)
            {
                string phone = reservation.DutyMember.Contact;
                string name = reservation.PosterName;
                int ID = reservation.Id;
                string message = SMSReservationClosed.Replace("#id#", ID + "").Replace("#name#", name);
                return SendSMSAsync(phone, message);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        // 用于向诊所人员发送短信
        // 通知申请被用户主动取消
        public Task SendReservationCancelledAsync(ReservationDetail reservation)
        {
            if (reservation.DutyMember != null && SMSReservationCancelled != null)
            {
                string phone = reservation.DutyMember.Contact;
                string name = reservation.PosterName;
                int ID = reservation.Id;
                string message = SMSReservationCancelled.Replace("#id#", ID + "").Replace("#name#", name);
                return SendSMSAsync(phone, message);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        // 用于向诊所人员发送短信
        // 当预约更改时通知受理该问题的人员
        public Task SendReservationUpdatedAsync(ReservationDetail reservation)
        {
            if (reservation.DutyMember != null && SMSReservationUpdated != null)
            {
                string phone = reservation.DutyMember.Contact;
                string name = reservation.PosterName;
                int ID = reservation.Id;
                string message = SMSReservationUpdated.Replace("#id#", ID + "").Replace("#name#", name);
                return SendSMSAsync(phone, message);
            }
            else
            {
                return Task.CompletedTask;
            }
        }


        // 发送短信底层
        private Task SendSMSAsync(string phone, string message)
        {
            return Task.Run(async () =>
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(smsurl);
                string data_send_sms = "apikey=" + smsapikey + "&mobile=" + phone + "&text=" + message;
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
