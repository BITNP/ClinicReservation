using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models.Data
{
    public enum GroupAction
    {
        CreateModifyReservation = 0,
        ManageAllReservations = 1,
        AcceptReservation = 2,
        SendBoardMessageToAllReservations = 3,
        SendBoardMessageOnlyRelated = 4,
        SendHiddenBoardMessage = 5,
        ModifyGroups = 6,
        ChangeServiceState = 7,
        ViewUsers = 8,

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
