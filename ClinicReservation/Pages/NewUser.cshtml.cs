using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using ClinicReservation.Services.Database;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(failedAction: AuthenticationFailedAction.Return401)]
    public class NewUserModel : CultureMatchingPageModel
    {
        private readonly DataDbContext dbContext;
        private readonly ICodeMatchingService codeMatching;
        private readonly IDbQuery dbQuery;

        public IEnumerable<Department> Departments { get; private set; }

        public NewUserFormModel UserModel { get; private set; }

        public NewUserModel(DataDbContext dbContext, ICodeMatchingService codeMatching, IDbQuery dbQuery)
        {
            this.dbContext = dbContext;
            this.codeMatching = codeMatching;
            this.dbQuery = dbQuery;
        }

        public IActionResult OnGet([FromServices] IAuthenticationResult authResult)
        {
            User user = dbQuery.TryGetUser(authResult.User);
            if (user == null)
            {
                UserModel = new NewUserFormModel()
                {
                    Department = Constants.DEFAULT_DEPARTMENT_CODE
                };
            }
            else if (user.IsPersonalInformationFilled)
            {
                // forbidden
                // create a profile more than once is not allowed
                return Unauthorized();
            }
            else
            {
                UserModel = new NewUserFormModel()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Department = Constants.DEFAULT_DEPARTMENT_CODE
                };
            }
            Departments = dbContext.Departments;
            codeMatching.Match(Departments);
            return Page();
        }

        public IActionResult OnPost([FromServices] IAuthenticationResult authResult, [FromForm] NewUserFormModel model)
        {
            model.Nullify();
            if (TryValidateModel(model))
            {
                string username = authResult.User.Name;
                User user = dbQuery.TryGetUserByName(username);
                if (user == null)
                {
                    user = new User()
                    {
                        Username = username,
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        IM = model.IM,
                        GitHub = model.GitHub,
                        Department = model.DepartmentInstance
                    };
                    user.Groups = new List<UserGroupUser>();
                    SetUserGroup(user, model.Code);
                    dbQuery.AddUser(user);
                    dbQuery.SaveChanges();
                }
                else if (user.IsPersonalInformationFilled)
                {
                    // forbidden
                    // create a profile more than once is not allowed
                    return new BadRequestResult();
                }
                else
                {
                    user.Phone = model.Phone;
                    user.IM = model.IM;
                    user.GitHub = model.GitHub;
                    user.Department = model.DepartmentInstance;
                    user.IsPersonalInformationFilled = true;
                    SetUserGroup(user, model.Code);
                    EntityEntry<User> entry = dbQuery.GetDbEntry(user);
                    entry.State = EntityState.Modified;
                    dbQuery.SaveChanges();
                }
                return Redirect("/board");
            }
            SetErrorMessage(model);
            return Page();
        }

        private void SetErrorMessage(NewUserFormModel model)
        {
        }

        private void SetUserGroup(User user, string code)
        {
            UserGroup normalGroup = dbQuery.TryGetNormalUserGroup();
            if (!user.Groups.Any(x => x.GroupId == normalGroup.Id))
            {
                user.Groups.Add(new UserGroupUser()
                {
                    User = user,
                    Group = dbQuery.TryGetNormalUserGroup()
                });
            }

            // add the user to a group according to the code
            if (string.IsNullOrWhiteSpace(code))
                return;

            string[] groupCodes = code.Split(';');
        }
    }
}