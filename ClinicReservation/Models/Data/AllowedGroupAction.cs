using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models.Data
{
    public enum GroupAction
    {
        CreateModifyReservation,
        ViewAllReservations,
        AcceptReservation,
        SendBoardMessageToAllReservations,
        SendBoardMessageOnlyRelated,
        SendHiddenBoardMessage
    }

    public class AllowedGroupAction
    {
        [Required]
        public GroupAction Action { get; set; }

        [Required]
        public int GroupId { get; set; }
        
        public UserGroup Group { get; set; }
    }
}
