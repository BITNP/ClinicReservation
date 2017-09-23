using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClinicReservation.Models.Reservation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ClinicReservation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClinicReservation.Services;
using Microsoft.AspNetCore.Http;
using ClinicReservation.Helpers;
using DNTCaptcha.Core.Contracts;

namespace ClinicReservation.Controllers
{
    public class ReservationController : LocalizedViewFindableController
    {
        public const int ITEMS_PER_PAGE = 10;

        private readonly ReservationDbContext db;
        private readonly NPOLJwtTokenService tokenservice;
        private readonly SMSService smsService;
        private readonly CultureContext cultureContext;
        private readonly ICaptchaProtectionProvider captchaProtectionProvider;
        private readonly IHumanReadableIntegerProvider humanReadableIntegerProvider;
        private TimeSpan cancelClosedTimeout = TimeSpan.FromDays(7);
        private static Dictionary<string, int> ACTION_CODES { get; } = new Dictionary<string, int>()
        {
            ["cancel"] = 100,
            ["restore"] = 125,
            ["submitmessage"] = 166,
            ["modify"] = 177,
            ["stop"] = 189,
            ["viewmsg"] = 190,
            ["complete"] = 340,
        };

        public ReservationController(ReservationDbContext dbcontext, NPOLJwtTokenService tokenservice, SMSService smsService, CultureContext cultureContext,
            ICaptchaProtectionProvider captchaProtectionProvider, IHumanReadableIntegerProvider humanReadableIntegerProvider) : base(cultureContext)
        {
            db = dbcontext;
            this.tokenservice = tokenservice;
            this.smsService = smsService;
            this.cultureContext = cultureContext;
            this.captchaProtectionProvider = captchaProtectionProvider;
            this.humanReadableIntegerProvider = humanReadableIntegerProvider;
        }

        // 在线预约首页
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Language(string source)
        {
            ViewData["Source"] = source ?? "/";
            return View();
        }


        // 新建预约请求页面
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["SchoolTypes"] = db.SchoolTypes.OrderBy(type => type.Id);
            ViewData["ProblemTypes"] = db.ProblemTypes.OrderBy(type => type.Id);
            ViewData["LocationTypes"] = db.LocationTypes.OrderBy(type => type.Id);
            return View();
        }

        // 查看预约信息
        [HttpPost]
        public IActionResult Detail(string id, string phone)
        {
            phone = phone ?? "";
            ReservationDetail _detail = VerifyReservationDetail(id, phone);

            if (_detail == null)
                return RedirectToAction(nameof(Index));

            SetSessionTicket(id, phone);
            return RedirectToActionPermanent(nameof(Detail));
        }
        [HttpGet]
        public IActionResult Detail()
        {
            string id, phone;
            if (TempData.ContainsKey("id") && TempData.ContainsKey("phone"))
            {
                id = TempData["id"] as string;
                phone = TempData["phone"] as string;
            }
            else
                GetSessionTicket(out id, out phone);
            ReservationDetail _detail = VerifyReservationDetail(id, phone);
            if (_detail == null)
                return RedirectToAction(nameof(Index));

            db.Entry(_detail).EnsureReferencesLoaded(true);

            if (_detail.ReservationBoardMessages.Count > 0)
            {
                List<ReservationBoardMessage> msgs = _detail.ReservationBoardMessages.Where(msg => msg.IsPublic == true).OrderByDescending(msg => msg.PostedTime).Take(5).ToList();
                foreach (var msg in msgs)
                {
                    db.Entry(msg).EnsureReferencesLoaded(false);
                }
                ViewData["BoardMessages"] = msgs;
            }
            else
                ViewData["BoardMessages"] = new List<ReservationBoardMessage>();

            // 超期自动关闭
            if (_detail.State == ReservationState.Cancelled && (DateTimeHelper.GetBeijingTime() - _detail.ActionDate) > cancelClosedTimeout)
            {
                _detail.State = ReservationState.ClosedWithoutComplete;
                _detail.ActionDate = _detail.ActionDate + cancelClosedTimeout;
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;
                db.SaveChanges();
                entry.Reload();
                _detail = entry.Entity;
            }
            ViewData["token"] = EncryptDetailCredential(_detail);
            ViewData["actionCode"] = ACTION_CODES;
            if (TempData.ContainsKey("showhint"))
            {
                TempData.Remove("showhint");
                ViewData["ShowHint"] = true;
            }
            else
                ViewData["ShowHint"] = false;

            SetSessionTicket(id, phone);
            return View(_detail);
        }

        // 处理提交预约请求
        [HttpPost]
        public IActionResult Create(string postername, string posterphone, string posteremail, string posterqq, string posterschool, string problemtype, string problemdetail, string location, string bookdate, string captchaText, string captchaToken)
        {
            string _postername = postername ?? "";
            string _posterphone = posterphone ?? "";
            string _posteremail = posteremail ?? "";
            string _posterqq = posterqq ?? "";

            if (!IsCaptchaValidate(captchaText, captchaToken))
            {
                ViewData["CaptchaError"] = true;
                ViewData["Name"] = _postername;
                ViewData["Phone"] = _posterphone;
                ViewData["Email"] = _posteremail;
                ViewData["QQ"] = _posterqq;
                ViewData["ProblemDetail"] = problemdetail;
                ViewData["School"] = posterschool;
                ViewData["ProblemType"] = problemtype;
                ViewData["Location"] = location;
                ViewData["BookDate"] = bookdate;
                ViewData["SchoolTypes"] = db.SchoolTypes.OrderBy(type => type.Id);
                ViewData["ProblemTypes"] = db.ProblemTypes.OrderBy(type => type.Id);
                ViewData["LocationTypes"] = db.LocationTypes.OrderBy(type => type.Id);
                return View();
            }

            DateTime _reservationDate;
            DateTime now = DateTimeHelper.GetBeijingTime();

            SchoolType _schoolType = db.SchoolTypes.FirstOrDefault(item => item.Name == posterschool);
            ProblemType _problemType = db.ProblemTypes.FirstOrDefault(item => item.Name == problemtype);
            LocationType _locationType = db.LocationTypes.FirstOrDefault(item => item.Name == location);

            if (_schoolType != null && _problemType != null && _locationType != null &&
                DateTime.TryParse(bookdate, out _reservationDate) == true)
            {
                _reservationDate = new DateTime(_reservationDate.Year, _reservationDate.Month, _reservationDate.Day, 23, 59, 59);
                ReservationDetail detail = new ReservationDetail()
                {
                    PosterName = _postername,
                    PosterPhone = _posterphone,
                    PosterEmail = _posteremail,
                    PosterQQ = _posterqq,
                    PosterSchoolType = _schoolType,
                    LocationType = _locationType,
                    ProblemType = _problemType,
                    Detail = problemdetail,
                    ActionDate = now,
                    CreateDate = now,
                    ModifiedDate = now,
                    ReservationDate = _reservationDate,
                    State = ReservationState.NewlyCreated
                };
                EntityEntry<ReservationDetail> entry = db.ReservationDetails.Add(detail);
                db.SaveChanges();
                smsService.SendCreationSuccessAsync(entry.Entity, cultureContext.Culture);
                smsService.SendReservationCreatedAsync(entry.Entity);
                TempData["id"] = entry.Entity.Id.ToString();
                TempData["phone"] = entry.Entity.GetShortenPhone();
                TempData["showhint"] = true;
                return RedirectToAction(nameof(Detail));
            }
            else
            {

            }
            return View();
        }

        [HttpPost]
        public IActionResult SubmitAction(string id, string phone, string token, string action, string content)
        {
            phone = phone ?? "";
            ReservationDetail _detail = VerifyReservationDetailWithTicket(id, phone, token);
            if (_detail == null)
                return RedirectToActionPermanent(nameof(Index));

            TempData["id"] = id;
            TempData["phone"] = phone;
            TempData["token"] = token;

            // action 解析失败，不采取动作
            int _action;
            int rate = -1;
            if (int.TryParse(action, out _action) == false)
                return RedirectToActionPermanent(nameof(Detail));

            // action 解析失败，不采取动作
            IEnumerable<KeyValuePair<string, int>> actpairs = ACTION_CODES.Where(pair => pair.Value == _action);
            if (actpairs.Count() == 0)
            {
                int complete_base = ACTION_CODES["complete"];
                int rating = _action - complete_base;
                if (rating >= 0 && rating <= 4)
                    rate = rating;
                else
                    return RedirectToActionPermanent(nameof(Detail));
            }

            string act;
            if (rate >= 0)
                act = "complete";
            else
                act = actpairs.First().Key;

            switch (act)
            {
                case "cancel":
                    return ActionCancel(_detail);
                case "restore":
                    return ActionRestore(_detail);
                case "submitmessage":
                    return ActionSubmitMessage(_detail, content ?? "");
                case "modify":
                    return ActionModify();
                case "stop":
                    return ActionStop(_detail);
                case "viewmsg":
                    return ActionViewMessage(_detail);
                case "complete":
                    return ActionComplete(_detail, rate - 2, content ?? "");
                default:
                    return RedirectToActionPermanent(nameof(Detail));
            }
        }

        [HttpPost]
        public IActionResult ViewMessage(string id, string phone, int page = 1)
        {
            TempData["id"] = id;
            TempData["phone"] = phone;
            TempData["page"] = page;
            return RedirectToActionPermanent(nameof(ViewMessage));

            ReservationDetail _detail = VerifyReservationDetail(id, phone);

            if (_detail == null)
                return RedirectToAction(nameof(Index));
            db.Entry(_detail).EnsureReferencesLoaded(true);

            int total = 1;
            int maxpage = 1;
            if (_detail.ReservationBoardMessages != null)
            {
                IEnumerable<ReservationBoardMessage> msgs = _detail.ReservationBoardMessages.Where(item => item.IsPublic == true);
                total = msgs.Count();
                maxpage = (int)Math.Ceiling(total * 1.0 / ITEMS_PER_PAGE);
                if (maxpage <= 0) maxpage = 1;
                if (page >= maxpage)
                    page = maxpage;
                page--;
                if (page < 0) page = 0;
                msgs = msgs.OrderByDescending(item => item.PostedTime).Skip(page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
                foreach (ReservationBoardMessage msg in msgs)
                    db.Entry(msg).EnsureReferencesLoaded(false);
                ViewData["Messages"] = msgs;
            }
            else
            {
                ViewData["Messages"] = new List<ReservationBoardMessage>();
                page = 0;
            }
            ViewData["Page"] = page;
            ViewData["MaxPage"] = maxpage;
            ViewData["id"] = id;
            ViewData["phone"] = phone;
            return View();
        }

        [HttpGet]
        public IActionResult ViewMessage(int page = 1)
        {
            string id, phone;
            if (TempData.ContainsKey("id") && TempData.ContainsKey("phone"))
            {
                id = TempData["id"] as string;
                phone = TempData["phone"] as string;
                if (TempData.ContainsKey("page"))
                    page = (int)TempData["page"];
            }
            else
                GetSessionTicket(out id, out phone);
            ReservationDetail _detail = VerifyReservationDetail(id, phone);
            if (_detail == null)
                return RedirectToAction(nameof(Index));

            db.Entry(_detail).EnsureReferencesLoaded(true);

            int total = 1;
            int maxpage = 1;
            if (_detail.ReservationBoardMessages != null)
            {
                IEnumerable<ReservationBoardMessage> msgs = _detail.ReservationBoardMessages.Where(item => item.IsPublic == true);
                total = msgs.Count();
                maxpage = (int)Math.Ceiling(total * 1.0 / ITEMS_PER_PAGE);
                if (maxpage <= 0) maxpage = 1;
                if (page >= maxpage)
                    page = maxpage;
                page--;
                if (page < 0) page = 0;
                msgs = msgs.OrderByDescending(item => item.PostedTime).Skip(page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
                foreach (ReservationBoardMessage msg in msgs)
                    db.Entry(msg).EnsureReferencesLoaded(false);
                ViewData["Messages"] = msgs;
            }
            else
            {
                ViewData["Messages"] = new List<ReservationBoardMessage>();
                page = 0;
            }
            ViewData["Page"] = page;
            ViewData["MaxPage"] = maxpage;
            ViewData["id"] = id;
            ViewData["phone"] = phone;
            return View();
        }

        private IActionResult ActionCancel(ReservationDetail _detail)
        {
            if (_detail == null)
                return RedirectToActionPermanent(nameof(Index));

            if (_detail.State == ReservationState.NewlyCreated || _detail.State == ReservationState.Answered)
            {
                _detail.State = ReservationState.Cancelled;
                _detail.ActionDate = DateTimeHelper.GetBeijingTime();
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;
                db.SaveChanges();
                smsService.SendReservationCancelledAsync(entry.Entity);
            }
            return RedirectToActionPermanent(nameof(Detail));
        }
        private IActionResult ActionStop(ReservationDetail _detail)
        {
            if (_detail == null)
                return RedirectToActionPermanent(nameof(Index));

            if (_detail.State == ReservationState.Cancelled)
            {
                _detail.State = ReservationState.ClosedWithoutComplete;
                _detail.ActionDate = DateTimeHelper.GetBeijingTime();
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;
                db.SaveChanges();
                smsService.SendReservationClosedAsync(entry.Entity);
            }
            return RedirectToActionPermanent(nameof(Detail));
        }
        private IActionResult ActionRestore(ReservationDetail _detail)
        {
            if (_detail == null)
                return RedirectToActionPermanent(nameof(Index));

            if (_detail.State == ReservationState.Cancelled)
            {
                _detail.State = ReservationState.NewlyCreated;
                _detail.ActionDate = DateTimeHelper.GetBeijingTime();
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToActionPermanent(nameof(Detail));
        }
        private IActionResult ActionSubmitMessage(ReservationDetail _detail, string message)
        {
            if (message.Length > 0)
            {
                ReservationBoardMessage boardmessage = new ReservationBoardMessage()
                {
                    DutyMember = null,
                    Message = message,
                    PostedTime = DateTimeHelper.GetBeijingTime(),
                    ReservationDetail = _detail,
                    IsPublic = true
                };
                db.ReservationBoardMessages.Add(boardmessage);
                db.SaveChanges();

            }
            return RedirectToActionPermanent(nameof(Detail));
        }
        private IActionResult ActionModify()
        {
            return RedirectToActionPermanent(nameof(Modify));
        }
        private IActionResult ActionViewMessage(ReservationDetail _detail)
        {
            return RedirectToActionPermanent(nameof(ViewMessage));
        }
        private IActionResult ActionComplete(ReservationDetail _detail, int rate, string content)
        {
            if (_detail == null)
                return RedirectToActionPermanent(nameof(Index));

            if (_detail.State == ReservationState.Answered)
            {
                _detail.State = ReservationState.Completed;
                _detail.ActionDate = DateTimeHelper.GetBeijingTime();
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;

                ServiceFeedback fb = new ServiceFeedback()
                {
                    Content = content,
                    Rate = rate,
                    ReservationDetail = _detail
                };
                db.ServiceFeedBacks.Add(fb);
                db.SaveChanges();
            }
            return RedirectToActionPermanent(nameof(Detail));
        }

        [HttpPost]
        private IActionResult Modify(string id, string phone, string token)
        {
            ReservationDetail _detail = VerifyReservationDetailWithTicket(id, phone, token);
            if (_detail == null)
                return RedirectToAction(nameof(Index));

            NavigationPropertyHelper.EnsureReferencesLoaded(db.Entry(_detail), false);

            ViewData["SchoolTypes"] = db.SchoolTypes.OrderBy(type => type.Id);
            ViewData["ProblemTypes"] = db.ProblemTypes.OrderBy(type => type.Id);
            ViewData["LocationTypes"] = db.LocationTypes.OrderBy(type => type.Id);
            ViewData["id"] = id;
            ViewData["phone"] = phone;
            ViewData["token"] = token;
            return View(_detail);
        }
        [HttpGet]
        public IActionResult Modify()
        {
            object id, phone, token;
            if (TempData.TryGetValue("id", out id) == true &&
                TempData.TryGetValue("phone", out phone) == true &&
                TempData.TryGetValue("token", out token) == true)
            {
                return Modify(id as string, phone as string, token as string);
            }
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SubmitModify(string id, string phone, string token, string postername, string posterphone, string posteremail, string posterqq, string posterschool, string problemtype, string problemdetail, string location, string bookdate)
        {
            ReservationDetail _detail = VerifyReservationDetailWithTicket(id, phone, token);
            if (_detail == null)
                return RedirectToAction(nameof(Index));

            DateTime _reservationDate;
            DateTime now = DateTimeHelper.GetBeijingTime();

            SchoolType _schoolType = (from school in db.SchoolTypes where school.Name == posterschool select school).FirstOrDefault();
            ProblemType _problemType = (from problem in db.ProblemTypes where problem.Name == problemtype select problem).FirstOrDefault();
            LocationType _locationType = (from loc in db.LocationTypes where loc.Name == location select loc).FirstOrDefault();

            string _postername = postername ?? "";
            string _posterphone = posterphone ?? "";
            string _posteremail = posteremail ?? "";
            string _posterqq = posterqq ?? "";

            if (_schoolType != null && _problemType != null && _locationType != null &&
                DateTime.TryParse(bookdate, out _reservationDate) == true)
            {
                _reservationDate = new DateTime(_reservationDate.Year, _reservationDate.Month, _reservationDate.Day, 23, 59, 59);
                _detail.PosterName = _postername;
                _detail.PosterPhone = _posterphone;
                _detail.PosterEmail = _posteremail;
                _detail.PosterQQ = _posterqq;
                _detail.PosterSchoolType = _schoolType;
                _detail.LocationType = _locationType;
                _detail.ProblemType = _problemType;
                _detail.Detail = problemdetail;
                _detail.ModifiedDate = now;
                _detail.ReservationDate = _reservationDate;
                EntityEntry<ReservationDetail> entry = db.Entry(_detail);
                entry.State = EntityState.Modified;
                db.SaveChanges();
                entry.Reload();
                smsService.SendReservationUpdatedAsync(entry.Entity);
                TempData["id"] = entry.Entity.Id.ToString();
                TempData["phone"] = entry.Entity.GetShortenPhone();
                return RedirectToActionPermanent(nameof(Detail));
            }
            else
            {

            }
            return View();
        }


        private void SetSessionTicket(string id, string phone)
        {
            byte[] idval = Encoding.UTF8.GetBytes(id);
            byte[] phoneval = Encoding.UTF8.GetBytes(phone);
            HttpContext.Session.Set("id", idval);
            HttpContext.Session.Set("phone", phoneval);
        }
        private void GetSessionTicket(out string id, out string phone)
        {
            byte[] idval;
            byte[] phoneval;
            if (HttpContext.Session.TryGetValue("id", out idval) == false)
                id = "";
            else
                id = Encoding.UTF8.GetString(idval);
            if (HttpContext.Session.TryGetValue("phone", out phoneval) == false)
                phone = "";
            else
                phone = Encoding.UTF8.GetString(phoneval);
        }

        private ReservationDetail VerifyReservationDetail(string id, string phone)
        {
            int _id;
            if (int.TryParse(id, out _id) == false)
                return null;

            ReservationDetail _detail = (from detail in db.ReservationDetails
                                         where (detail.Id == _id && ((detail.PosterPhone.Length == 0 && (phone == null || phone.Length == 0)) || (detail.PosterPhone.Length > 0 && phone != null && phone.Length > 0 && detail.PosterPhone.EndsWith(phone))))
                                         select detail).FirstOrDefault();
            return _detail;
        }
        private ReservationDetail VerifyReservationDetailWithTicket(string id, string phone, string token)
        {
            Dictionary<string, string> option = null;
            try
            {
                option = tokenservice.Decrypt(token);
            }
            catch
            {
                option = null;
            }

            string dicid;
            string dicphone;
            if (option == null)
                return null;

            if (option.TryGetValue("id", out dicid) == false)
                return null;

            if (option.TryGetValue("phone", out dicphone) == false)
                dicphone = "";
            if (dicid != id || dicphone != phone)
                return null;

            return VerifyReservationDetail(dicid, dicphone);
        }
        private string EncryptDetailCredential(ReservationDetail detail)
        {
            Dictionary<string, string> option = new Dictionary<string, string>()
            {
                ["id"] = detail.Id.ToString(),
                ["phone"] = detail.GetShortenPhone()
            };
            return tokenservice.Encrypt(option);
        }

        private bool IsCaptchaValidate(string captchaText, string captchaToken)
        {
            string token = captchaProtectionProvider.Decrypt(captchaToken);
            int number;
            if (!int.TryParse(captchaText, out number))
                return false;
            string text = humanReadableIntegerProvider.NumberToText(number, DNTCaptcha.Core.Providers.Language.English);
            return text == token;
        }

    }
}
