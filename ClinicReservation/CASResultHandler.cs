using AuthenticationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace ClinicReservation
{
    public class CASResultHandler : ICASResponseHandler
    {
        private static TimeSpan SYNC_TIMEOUT { get; } = TimeSpan.FromHours(12);
        private static Regex COLON_REPLACER { get; } = new Regex(@"(?<=\</?cas):");

        private static XPathExpression USER_EXP { get; } = XPathExpression.Compile("cas-user");
        private static XPathExpression SUCCESS_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess");
        private static XPathExpression FAILURE_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationFailure/@code");
        private static XPathExpression EMAIL_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess/cas-attributes/cas-email");
        private static XPathExpression REALNAME_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess/cas-attributes/cas-real_name");
        private static XPathExpression OAUTH_PROFILE_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess/cas-attributes/cas-oauth_profile");
        private static XPathExpression AVATAR_EXP { get; } = XPathExpression.Compile("/cas-serviceResponse/cas-authenticationSuccess/cas-attributes/cas-avatar");

        private readonly ICASOption option;
        private readonly IDbQuery query;

        public CASResultHandler(ICASOption option, IDbQuery query)
        {
            this.option = option;
            this.query = query;
        }

        public IUser Invoke(HttpContext httpContext, string message, string actionUrl, out string redirectUrl)
        {
            message = COLON_REPLACER.Replace(message, "-");
            TextReader textReader = new StringReader(message);
            XPathDocument document = new XPathDocument(textReader);
            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(SUCCESS_EXP);
            if (iterator.Count > 0)
            {
                iterator.MoveNext();
                navigator = iterator.Current;
                string user = navigator.SelectSingleNode(USER_EXP).Value;

                httpContext.Session.SetString(option.SessionName, user);
                redirectUrl = actionUrl;

                string email = null;
                string real_name = null;
                string oauth = null;
                string avatar = null;
                iterator = navigator.Select(EMAIL_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    email = iterator.Current.Value;
                }
                iterator = navigator.Select(REALNAME_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    real_name = iterator.Current.Value;
                }
                iterator = navigator.Select(OAUTH_PROFILE_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    oauth = iterator.Current.Value;
                    if (oauth == "null")
                        oauth = null;
                }
                iterator = navigator.Select(AVATAR_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    avatar = iterator.Current.Value;
                    if (avatar == "null")
                        avatar = null;
                }
                if (avatar == null)
                    avatar = Constants.DEFAULT_AVATAR;
                OnSuccess(user, email, real_name, avatar, oauth);
                return null;
            }
            else
            {
                string code = "UNKNOWN_FAILURE";
                iterator = navigator.Select(FAILURE_EXP);
                if (iterator.Count > 0)
                {
                    iterator.MoveNext();
                    code = iterator.Current.Value;
                }
            }
            redirectUrl = null;
            return null;
        }

        private void OnSuccess(string username, string email, string real_name, string avatar, string oauth)
        {
            User user = query.TryGetUserByName(username);
            DateTime now = DateTime.Now;
            if (user == null)
            {
                user = new User()
                {
                    Username = username,
                    Department = query.TryGetDefaultDepartment(),
                    Email = email,
                    Avatar = avatar,
                    Name = real_name,
                    IsPersonalInformationFilled = false
                };
                UserGroupUser groupAssociation = new UserGroupUser()
                {
                    User = user,
                    Group = query.TryGetNormalUserGroup()
                };
                user.LastSyncTime = now;
                user.Groups = new List<UserGroupUser>();
                user.Groups.Add(groupAssociation);
                query.AddUser(user);
                query.SaveChanges();
            }
            else
            {

                if ((now - user.LastSyncTime) > SYNC_TIMEOUT)
                {
                    // sync user profile
                    if (user.Email != email)
                        user.Email = email;
                    if (user.Name != real_name)
                        user.Name = real_name;
                    if (user.Avatar != avatar)
                        user.Avatar = avatar;
                    user.LastSyncTime = now;
                    query.GetDbEntry(user).State = EntityState.Modified;
                    query.SaveChanges();
                }
            }
        }
    }
}
