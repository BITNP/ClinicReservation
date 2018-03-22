//#define NO_MEMBER_LOGIN_REQUIRED_FOR_CREATE_MEMBER

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ClinicReservation.Services;
//using ClinicReservation.Models;
//using ClinicReservation.Models.Reservation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Hosting;
//using System.IO;
//using ClinicReservation.Helpers;

//namespace ClinicReservation.Controllers
//{
//    public class NameParameter
//    {
//        public string Name { get; set; }
//    }
//    public class MemberController : Controller
//    {
//        public const int ITEMS_PER_PAGE = 10;

//        private readonly SMSService smsService;
//        private readonly ReservationDbContext db;
//        private readonly NPOLJwtTokenService tokenservice;
//        private readonly ServiceConfig serviceConfig;
//        private readonly string membersPath;

//        public MemberController(IHostingEnvironment env, ReservationDbContext dbcontext, NPOLJwtTokenService tokensrv, ServiceConfig serviceConfig, SMSService smsService)
//        {
//            db = dbcontext;
//            tokenservice = tokensrv;
//            this.serviceConfig = serviceConfig;
//            this.smsService = smsService;
//            membersPath = Path.Combine(env.WebRootPath, "images", "members");
//        }

//        [HttpGet]
//        public IActionResult RenewTicket()
//        {
//            ServiceConfig.ReadTicket();
//            return NoContent();
//        }

//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Main(string name, string pwd)
//        {
//            DutyMember member = (from mem in db.DutyMembers where mem.LoginName == name && mem.LoginPwd == pwd select mem).FirstOrDefault();
//            if (member == null)
//                return RedirectToAction(nameof(Index));

//            // add tokens
//            SetDutyMemberLogin(member);

//            return RedirectToAction(nameof(ViewDetails));
//        }

//        [HttpPost]
//        public IActionResult CreateMember(string ticket, string name, string contact, string loginname, string password, string grade, Sexual sexual, string school, IList<IFormFile> userpic)
//        {
//#if NO_MEMBER_LOGIN_REQUIRED_FOR_CREATE_MEMBER
//            if ((ticket == null || ticket.Length <= 0) ||
//                (name == null || name.Length <= 0) ||
//                (contact == null || contact.Length <= 0) ||
//                (loginname == null || loginname.Length <= 0) ||
//                (password == null || password.Length != 32) ||
//                (school == null) ||
//                (userpic.Count <= 0))
//#else
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null ||
//                (ticket == null || ticket.Length <= 0) ||
//                (name == null || name.Length <= 0) ||
//                (contact == null || contact.Length <= 0) ||
//                (loginname == null || loginname.Length <= 0) ||
//                (password == null || password.Length != 32) ||
//                (school == null) ||
//                (userpic.Count <= 0))
//#endif

//                return new JsonResult(new { result = false, reason = "信息填写不完整" });
//            if (ticket != serviceConfig.RegisterationTicket)
//                return new JsonResult(new { result = false, reason = "创建秘钥认证失败" });

//            SchoolType sch = (from s in db.SchoolTypes where s.Name == school select s).FirstOrDefault();
//            if (sch == null)
//                return new JsonResult(new { result = false, reason = "学校类型错误" });
//            DutyMember check = (from m in db.DutyMembers where m.LoginName == loginname select m).FirstOrDefault();
//            if (check != null)
//                return new JsonResult(new { result = false, reason = "登录名已存在" });

//            string path = Path.Combine(membersPath, loginname + ".jpg");
//            FileStream stream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
//            stream.SetLength(0);
//            userpic[0].CopyTo(stream);
//            stream.Flush();
//            stream.Dispose();

//            DutyMember memcreate = new DutyMember()
//            {
//                Name = name,
//                LoginName = loginname,
//                LoginPwd = password,
//                Grade = grade,
//                Sexual = sexual,
//                School = sch,
//                Contact = contact,
//                IconName = loginname + ".jpg",
//            };
//            db.DutyMembers.Add(memcreate);
//            db.SaveChanges();
//            return new JsonResult(new { result = true });
//        }
//        [HttpGet]
//        public IActionResult CreateMember()
//        {
//#if !NO_MEMBER_LOGIN_REQUIRED_FOR_CREATE_MEMBER
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return RedirectToAction(nameof(Index));
//#endif
//            ViewData["SchoolTypes"] = db.SchoolTypes.OrderBy(type => type.Id);
//            return View();
//        }


//        [HttpPost]
//        public bool QueryLoginName([FromBody] NameParameter param)
//        {
//            if (param.Name == null || (param.Name = param.Name.Trim()).Length <= 0)
//                return false;
//            DutyMember check = db.DutyMembers.FirstOrDefault(item => item.LoginName == param.Name);
//            if (check == null)
//                return true;
//            else
//                return false;
//        }

//        [HttpGet]
//        public IActionResult ViewDetails(int id = -1, string filter = "uncompleted", int page = 1)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return RedirectToAction(nameof(Index));

//            ViewData["Member"] = member;
//            var details = from detail in db.ReservationDetails select detail;
//            int index = 0;
//            DateTime now = DateTimeHelper.GetBeijingTime();
//            bool showColor = false;
//            switch (filter)
//            {
//                case "uncompleted":
//                    index = 0;
//                    details = details.Where(detail => detail.ReservationDate >= now && (detail.State == ReservationState.Answered || detail.State == ReservationState.Cancelled || detail.State == ReservationState.NewlyCreated));
//                    break;
//                case "me":
//                    details = details.Where(detail => detail.DutyMember == member).OrderBy(detail => detail.ReservationDate);
//                    index = 1;
//                    break;
//                case "closed":
//                    details = details.Where(detail => detail.State == ReservationState.ClosedWithoutComplete).OrderByDescending(detail => detail.ActionDate);
//                    index = 2;
//                    break;
//                case "completed":
//                    details = details.Where(detail => detail.State == ReservationState.Completed).OrderByDescending(detail => detail.ActionDate);
//                    index = 3;
//                    break;
//                case "all":
//                default:
//                    index = 4;
//                    details = details.OrderBy(detail => detail.ReservationDate);
//                    showColor = true;
//                    break;
//            }
//            ViewData["Filter"] = index;
//            ViewData["ShowColor"] = showColor;
//            int total = details.Count();
//            int maxpage = (int)Math.Ceiling(total * 1.0 / ITEMS_PER_PAGE);
//            if (maxpage <= 0) maxpage = 1;
//            if (page >= maxpage)
//                page = maxpage;
//            page--;
//            if (page < 0) page = 0;
//            details = details.Skip(page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
//            if (id == -1)
//            {
//                ViewData["Current"] = null;
//                ViewData["Token"] = GetDutyMemberDetailToken(member, "-1");
//            }
//            else
//            {
//                ReservationDetail current = (from detail in db.ReservationDetails where detail.Id == id select detail).FirstOrDefault();
//                if (current == null)
//                    return RedirectToAction(nameof(ViewDetails), new { filter = filter, page = page });

//                db.Entry(current).EnsureReferencesLoaded(true);
//                IEnumerable<ReservationBoardMessage> messages = current.ReservationBoardMessages.OrderByDescending(item => item.PostedTime).Take(5);
//                foreach (ReservationBoardMessage msg in messages)
//                    db.Entry(msg).EnsureReferencesLoaded(false);
//                ViewData["Messages"] = messages;
//                ViewData["Current"] = current;
//                ViewData["Token"] = GetDutyMemberDetailToken(member, current.Id.ToString());
//            }
//            if (details.Count() > 0)
//            {
//                List<ReservationDetail> detaillist = details.ToList();
//                foreach (ReservationDetail detail in detaillist)
//                    db.Entry(detail).EnsureReferencesLoaded(false);
//                ViewData["Details"] = detaillist;
//            }
//            else
//                ViewData["Details"] = new List<ReservationDetail>();
//            ViewData["ShowMore"] = page < (maxpage - 1);
//            return View();
//        }

//        [HttpPost]
//        public IActionResult AcceptDetail(int id, string token)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });

//            if (VerifyDutyMemberDetailToken(member, id.ToString(), token) == false)
//                return new JsonResult(new { result = false });

//            ReservationDetail detail = (from d in db.ReservationDetails where d.Id == id select d).FirstOrDefault();
//            if (detail == null)
//                return new JsonResult(new { result = false });

//            if (detail.State != ReservationState.NewlyCreated)
//                return new JsonResult(new { result = false });

//            detail.State = ReservationState.Answered;
//            detail.DutyMember = member;
//            detail.ActionDate = DateTimeHelper.GetBeijingTime();
//            db.Entry(detail).State = EntityState.Modified;
//            db.SaveChanges();
//            smsService.SendAnsweredAsync(detail);
//            return new JsonResult(new { result = true });
//        }

//        [HttpPost]
//        public IActionResult CancelAcceptDetail(int id, string token)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });

//            if (VerifyDutyMemberDetailToken(member, id.ToString(), token) == false)
//                return new JsonResult(new { result = false });

//            ReservationDetail detail = (from d in db.ReservationDetails where d.Id == id select d).FirstOrDefault();
//            if (detail == null)
//                return new JsonResult(new { result = false });

//            if (detail.DutyMember != member || (detail.State != ReservationState.Answered && detail.State != ReservationState.Cancelled))
//                return new JsonResult(new { result = false });

//            if (detail.State == ReservationState.Answered)
//                detail.State = ReservationState.NewlyCreated;
//            else if (detail.State == ReservationState.Cancelled)
//                detail.State = ReservationState.Cancelled;

//            detail.DutyMember = null;
//            db.Entry(detail).State = EntityState.Modified;
//            db.SaveChanges();

//            return new JsonResult(new { result = true });
//        }

//        [HttpPost]
//        public IActionResult CompleteDetail(int id, string token)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });

//            if (VerifyDutyMemberDetailToken(member, id.ToString(), token) == false)
//                return new JsonResult(new { result = false });

//            ReservationDetail detail = (from d in db.ReservationDetails where d.Id == id select d).FirstOrDefault();
//            if (detail == null)
//                return new JsonResult(new { result = false });

//            if (detail.DutyMember == member && detail.State == ReservationState.Answered)
//            {
//                detail.State = ReservationState.Completed;
//                detail.ActionDate = DateTimeHelper.GetBeijingTime();
//                db.Entry(detail).State = EntityState.Modified;
//                db.SaveChanges();
//                return new JsonResult(new { result = true });
//            }
//            else
//                return new JsonResult(new { result = false });
//        }

//        [HttpPost]
//        public IActionResult CloseDetail(int id, string token)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });

//            if (VerifyDutyMemberDetailToken(member, id.ToString(), token) == false)
//                return new JsonResult(new { result = false });

//            ReservationDetail detail = (from d in db.ReservationDetails where d.Id == id select d).FirstOrDefault();
//            if (detail == null)
//                return new JsonResult(new { result = false });

//            switch (detail.State)
//            {
//                case ReservationState.NewlyCreated:
//                    detail.State = ReservationState.ClosedWithoutComplete;
//                    detail.ActionDate = DateTimeHelper.GetBeijingTime();
//                    db.Entry(detail).State = EntityState.Modified;
//                    db.SaveChanges();
//                    break;
//                case ReservationState.Answered:
//                case ReservationState.Cancelled:
//                    if (member != detail.DutyMember)
//                        return new JsonResult(new { result = false });

//                    detail.State = ReservationState.ClosedWithoutComplete;
//                    detail.ActionDate = DateTimeHelper.GetBeijingTime();
//                    db.Entry(detail).State = EntityState.Modified;
//                    db.SaveChanges();
//                    break;
//                case ReservationState.Completed:
//                case ReservationState.ClosedWithoutComplete:
//                default:
//                    break;
//            }
//            return new JsonResult(new { result = true });
//        }

//        [HttpPost]
//        public IActionResult AddDetailMessage(int id, string token, string message, bool ispublic)
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });

//            if (VerifyDutyMemberDetailToken(member, id.ToString(), token) == false)
//                return new JsonResult(new { result = false });

//            ReservationDetail detail = (from d in db.ReservationDetails where d.Id == id select d).FirstOrDefault();
//            if (detail == null)
//                return new JsonResult(new { result = false });

//            ReservationBoardMessage msg = new ReservationBoardMessage()
//            {
//                DutyMember = member,
//                Message = message,
//                PostedTime = DateTimeHelper.GetBeijingTime(),
//                ReservationDetail = detail,
//                IsPublic = ispublic
//            };
//            db.ReservationBoardMessages.Add(msg);
//            db.SaveChanges();

//            return new JsonResult(new { result = true });
//        }

//        /// <summary>
//        /// 用于保持登录状态
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        public IActionResult LoginHeartbeat()
//        {
//            DutyMember member = VerifyDutyMemberLogIn();
//            if (member == null)
//                return new JsonResult(new { result = false });
//            else
//                return new JsonResult(new { result = true });
//        }

//        public const string UUID_COOKIE_NAME = "uuid";
//        public const string LOGINTOKEN_COOKIE_NAME = "npauth";
//        public const int LOGIN_EXPIRE_MINUTES = 5;

//        private DutyMember VerifyDutyMemberLogIn()
//        {
//            string uid;
//            string token;
//            if (HttpContext.Request.Cookies.TryGetValue(UUID_COOKIE_NAME, out uid) == false)
//                return null;

//            if (HttpContext.Request.Cookies.TryGetValue(LOGINTOKEN_COOKIE_NAME, out token) == false)
//                return null;

//            Dictionary<string, string> keys = null;
//            try
//            {
//                keys = tokenservice.Decrypt(token);
//            }
//            catch
//            {
//                return null;
//            }

//            string _uid;
//            string username;
//            if (keys.TryGetValue("uid", out _uid) == false || keys.TryGetValue("name", out username) == false)
//                return null;

//            if (_uid != uid)
//                return null;
//            DutyMember member = (from mem in db.DutyMembers where mem.LoginName == username select mem).FirstOrDefault();

//            // 登录状态续命
//            DateTime expire = DateTime.Now.AddMinutes(LOGIN_EXPIRE_MINUTES);
//            HttpContext.Response.Cookies.Append(UUID_COOKIE_NAME, uid, new CookieOptions()
//            {
//                Expires = new DateTimeOffset(expire, TimeZoneInfo.Local.GetUtcOffset(expire)),
//            });
//            HttpContext.Response.Cookies.Append(LOGINTOKEN_COOKIE_NAME, token, new CookieOptions()
//            {
//                Expires = new DateTimeOffset(expire, TimeZoneInfo.Local.GetUtcOffset(expire)),
//            });
//            return member;
//        }
//        private void SetDutyMemberLogin(DutyMember member)
//        {
//            DateTime expire = DateTime.Now.AddMinutes(LOGIN_EXPIRE_MINUTES);

//            string uid = Guid.NewGuid().ToString();
//            HttpContext.Response.Cookies.Append(UUID_COOKIE_NAME, uid, new CookieOptions()
//            {
//                Expires = new DateTimeOffset(expire, TimeZoneInfo.Local.GetUtcOffset(expire)),
//            });

//            Dictionary<string, string> keys = new Dictionary<string, string>()
//            {
//                ["name"] = member.LoginName,
//                ["uid"] = uid
//            };
//            string token = tokenservice.Encrypt(keys);
//            HttpContext.Response.Cookies.Append(LOGINTOKEN_COOKIE_NAME, token, new CookieOptions()
//            {
//                Expires = new DateTimeOffset(expire, TimeZoneInfo.Local.GetUtcOffset(expire)),
//            });
//        }

//        private bool VerifyDutyMemberDetailToken(DutyMember member, string id, string token)
//        {
//            try
//            {
//                Dictionary<string, string> keys = tokenservice.Decrypt(token);
//                string _id;
//                string username;
//                if (keys.TryGetValue("id", out _id) == false || keys.TryGetValue("name", out username) == false)
//                    return false;

//                if (username == member.LoginName && id == _id)
//                    return true;
//                else
//                    return false;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//        private string GetDutyMemberDetailToken(DutyMember member, string id)
//        {
//            Dictionary<string, string> keys = new Dictionary<string, string>()
//            {
//                ["name"] = member.LoginName,
//                ["id"] = id
//            };
//            return tokenservice.Encrypt(keys);
//        }
//    }
//}
