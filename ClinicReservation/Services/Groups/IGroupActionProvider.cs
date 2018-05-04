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
        GroupAction[] AllActions { get; }
        GroupAction this[string key] { get; }
        string this[GroupAction action] { get; }

        bool TryGetAction(string key, out GroupAction action);
    }

    internal sealed class GroupActionProvider : IGroupActionProvider
    {
        private readonly Type actionType;
        private readonly string[] keys;
        private readonly GroupAction[] actions;
        private readonly Dictionary<string, GroupAction> keyActionMap;
        private readonly Dictionary<GroupAction, string> actionKeyMap;

        public int TotalActionCount => actions.Length;
        public string[] AllActionKeys => keys;
        public GroupAction[] AllActions => actions;

        public string this[GroupAction action] => actionKeyMap[action];
        public GroupAction this[string key] => keyActionMap[key];

        public GroupActionProvider()
        {
            actionType = typeof(GroupAction);
            Array values = Enum.GetValues(actionType);
            keyActionMap = new Dictionary<string, GroupAction>();
            actionKeyMap = new Dictionary<GroupAction, string>();
            foreach (GroupAction action in values)
            {
                string name = Enum.GetName(actionType, action);
                actionKeyMap.Add(action, name);
                keyActionMap.Add(name, action);
            }

            actions = keyActionMap.Values.ToArray();
            keys = keyActionMap.Keys.ToArray();
        }

        public bool TryGetAction(string key, out GroupAction action)
        {
            return keyActionMap.TryGetValue(key, out action);
        }
    }
}
