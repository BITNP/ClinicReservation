using ClinicReservation.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace ClinicReservation.Services.Groups
{
    internal sealed class GroupPromptResolver : IGroupPromptResolver
    {
        private SortedSet<string> codes;
        private readonly DataDbContext dbContext;

        public GroupPromptResolver(DataDbContext dbContext)
        {
            codes = new SortedSet<string>(dbContext.UserGroups.Select(group => group.PromptCode));
            this.dbContext = dbContext;
        }

        public IReadOnlyList<UserGroup> Resolve(string promptCode)
        {
            SortedSet<string> codes = new SortedSet<string>(promptCode.Split(';'));
            List<UserGroup> result = new List<UserGroup>();
            foreach (string code in codes)
            {
                if (string.IsNullOrWhiteSpace(code))
                    continue;

                result.Add(dbContext.UserGroups.First(group => group.PromptCode.Equals(code)));
            }
            return result;
        }
    }
}
