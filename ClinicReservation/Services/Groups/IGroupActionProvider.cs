using ClinicReservation.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Groups
{
    public interface IGroupActionProvider
    {
        int TotalActionCount { get; }
        string[] AllActionKeys { get; }
        GroupAction this[int index] { get; }
        string GetKey(int index);
        string GetKey(GroupAction action);
    }

    internal sealed class GroupActionProvider : IGroupActionProvider
    {
        private Type actionType;
        private string[] keys;
        private GroupAction[] actions;
        public int TotalActionCount => actions.Length;
        public string[] AllActionKeys => keys;
        public GroupAction this[int index] => actions[index];

        public GroupActionProvider()
        {
            actionType = typeof(GroupAction);
            Array values = Enum.GetValues(actionType);
            actions = values.OfType<GroupAction>().ToArray();
            keys = Enum.GetNames(actionType).ToArray();
        }

        public string GetKey(int index)
        {
            return keys[index];
        }

        public string GetKey(GroupAction action)
        {
            return Enum.GetName(actionType, action);
        }
    }
}
