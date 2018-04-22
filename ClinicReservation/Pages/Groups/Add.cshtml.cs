using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using AuthorizationCore;
using AuthorizationCore.Attributes;
using ClinicReservation.Authorizations;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages.Groups
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.KeepUnauthenticated)]
    [UserAuthorizationRequired(Policies.CanModifyGroups, AuthorizationFailedAction.KeepUnauthorized)]
    public class AddModel : PageModel
    {
        private readonly IDbQuery dbQuery;
        private readonly IAuthorizationResult authorization;

        public AddModel(IDbQuery dbQuery, IAuthorizationResult authorization)
        {
            this.dbQuery = dbQuery;
            this.authorization = authorization;
        }


        public IActionResult OnGet([FromForm] QueryGroupCodeFormModel model)
        {
            if (authorization.Succeeded(true) && ModelState.ValidationState == ModelValidationState.Valid)
                return new JsonResult(new { available = true });
            return new JsonResult(new { available = false });
        }

        public IActionResult OnPost([FromForm] AddGroupCodeFormModel model)
        {
            if (authorization.Succeeded(true) && ModelState.ValidationState == ModelValidationState.Valid)
            {
                string code = model.Code.Trim();
                string prompt = model.Promption;
                UserGroup group = new UserGroup()
                {
                    Code = code,
                    DefaultName = code,
                    PromptCode = prompt
                };
                dbQuery.AddGroup(group);
                dbQuery.SaveChanges();
            }
            return RedirectToPagePermanent("index");
        }
    }
}