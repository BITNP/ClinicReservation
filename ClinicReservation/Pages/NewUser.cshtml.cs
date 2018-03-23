using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet([FromServices] IAuthenticationResult authResult)
        {
            Departments = dbContext.Departments;
            codeMatching.Match(Departments);
        }

        public IActionResult OnPost([FromServices] IAuthenticationResult authResult, [FromForm] NewUserFormModel model)
        {
            model.Nullify();
            if (TryValidateModel(model))
            {
                string username = authResult.User.Name;
                User user = new User()
                {
                    Username = username,
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    IM = model.IM,
                    GitHub = model.GitHub
                };
                user.Department = dbQuery.TryGetDepartmentByCode(model.Department);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return Redirect("/board");
            }
            return Page();
        }
    }
}