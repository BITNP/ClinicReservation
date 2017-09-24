using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Reservation
{
    public class ReservationDetail
    {
        [Required, Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(64)]
        public string PosterName { get; set; }

        [MaxLength(20)]
        public string PosterPhone { get; set; }

        [MaxLength(64)]
        public string PosterEmail { get; set; }

        [MaxLength(64)]
        public string PosterQQ { get; set; }

        public SchoolType PosterSchoolType { get; set; }

        public ProblemType ProblemType { get; set; }

        [MaxLength(8)]
        public string LastUpdatedLanguage { get; set; }

        [MaxLength(512)]
        public string Detail { get; set; }

        public LocationType LocationType { get; set; }
    
        public ReservationState State { get; set; }

        public DutyMember DutyMember { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime CreateDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime ModifiedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReservationDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime ActionDate { get; set; }
        
        public ServiceFeedback Feedback { get; set; }

        public IList<ReservationBoardMessage> ReservationBoardMessages { get; set; }


        public string GetShortenPhone()
        {
            if (PosterPhone == null)
                return "";

            int begin = Math.Max(PosterPhone.Length - 4, 0);
            return PosterPhone.Substring(begin);
        }
        public string GetStatusString()
        {
            switch (State)
            {
                case ReservationState.NewlyCreated:
                    return string.Format("{0:yyyy/MM/dd HH:mm}创建", CreateDate);
                case ReservationState.Cancelled:
                    return "临时取消";
                case ReservationState.Answered:
                    return string.Format("由 {0} 受理", DutyMember.Name);
                case ReservationState.Completed:
                    return "已完成";
                case ReservationState.ClosedWithoutComplete:
                    return "永久关闭";
                default:
                    return "未知状态";
            }
        }
    }
}
