using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using ElYamanyDashboard.Models;
 using System.Data.Entity.Migrations;
 using Newtonsoft.Json;
using System.Net.Http.Headers;
 using ElYamanyDashboard.Utils;
using ElYamanyDashboard.Models.Views;
using System.Web.Script.Serialization;
using System.Text;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class UsersController : Controller
    {

        private ElYamanyContext db = new ElYamanyContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        UserCodeGenerator _userCodeGenerator = new UserCodeGenerator();
        // GET: Users
        public async Task<ActionResult> Index(int NewMembers=0,int NotActive=0)
        {
            List<User> users = new List<User>();
            if (NotActive>0)
            {
                users =await db.Users.Include(u => u.Area).Include(u => u.Gender).Where(i=>i.Enabled==false).ToListAsync();

            }
            else if (NewMembers>0)
            {
                users = await db.Users.Include(u => u.Area).Include(u => u.Gender).Where(i => i.CreationDate.Value.Month == DateTime.UtcNow.Month).ToListAsync();

            }
            else
            {
                users = await db.Users.Include(u => u.Area).Include(u => u.Gender).ToListAsync();

            }
            ViewBag.Total=users.Count;
            return View(users);
        }


        // GET: Users/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name");
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name");
            ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User user, HttpPostedFileBase Image, HttpPostedFileBase IdentityFrontImage, HttpPostedFileBase IdentityBackImage)
        {

            if (ModelState.IsValid)
            {
                var identityNumberFound = await GetUserByIdentityNumber(user.IdentityNumber);
                if (identityNumberFound!=null)
                {
                    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                    ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");

                    ViewBag.Error = "رقم الهوية مسجل من قبل";
                    return View(user);

                }
                var phoneNumberFound = await GetUserByPhoneNumber(user.PhoneNumber);
                if (phoneNumberFound != null)
                {
                    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                    ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");

                    ViewBag.Error = "رقم الهاتف مسجل من قبل";
                    return View(user);

                }
                if (!user.IdentityNumber.StartsWith("2"))
                {
                    if (!user.IdentityNumber.StartsWith("3"))
                    {
                        ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                        ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                        ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");

                        ViewBag.Image = Image;
                        ViewBag.Error = "يجب ان يكون بداية الرقم القومي 2 او 3";
                        return View(user);

                    }
                }

                user.ChequeNumber = 1;
                var DateOfBirth = user.DateOfBirth.ToShortDateString();
                var NationalId = user.IdentityNumber;
                var BDay = user.DateOfBirth.Day;
                var BMonth = user.DateOfBirth.Month;
                var BYear = Convert.ToInt32(user.DateOfBirth.Year.ToString().Substring(2, 2));
                var IYear = Convert.ToInt32(NationalId.Substring(1, 2));
                var IMonth = Convert.ToInt32(NationalId.Substring(3, 2));
                var IDay = Convert.ToInt32(NationalId.Substring(5, 2));
                if (BDay != IDay || BMonth != IMonth || IYear != BYear)
                {
                    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                    ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");

                    ViewBag.Error = "تاريخ الميلاد ورقم الهوية غير متطابقان ";
                    return View(user);

                }
                var UserCode = _userCodeGenerator.getNewCode();
                user.UserCode = UserCode;
                string Email = UserCode + "@ElYamany.com";
                var Identityuser = new ApplicationUser
                {
                    UserName = Email,
                    Email = Email,
                    PhoneNumber = user.PhoneNumber,
                };
                var result = await UserManager.CreateAsync(Identityuser, user.IdentityNumber);
                if (result.Succeeded)
                {
                    var IdentityUserData = await UserManager.FindByEmailAsync(Email);
                    await UserManager.AddToRoleAsync(IdentityUserData.Id, "User");

                    user.IdentityId = IdentityUserData.Id;
                    user.CreationDate = DateTime.UtcNow.AddHours(3);
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    if (Image != null)
                    {
                        await SetPhoto(Image, user.Id, "ProfileImage");
                    }
                    if (IdentityFrontImage != null)
                    {
                        await SetPhoto(IdentityFrontImage, user.Id, "IdentityFrontImage");
                    }
                    if (IdentityBackImage != null)
                    {
                        await SetPhoto(IdentityBackImage, user.Id, "IdentityBackImage");

                    }
                    return RedirectToAction("Details",new { id= user.Id});
                }
                else
                {

                    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                    ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");
                    ViewBag.Error = "Try again later";
                    return View(user);
                }
               
            }

            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
            ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");
            return View(user);
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            var data =await  db.Users.AsNoTracking().Where(i => i.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            return data;

        }

        public async Task<User> GetUserByIdentityNumber(string identityNumber)
        {
            var data = await db.Users.AsNoTracking().Where(i => i.IdentityNumber == identityNumber).FirstOrDefaultAsync();
            return data;
        }
        public async Task SetPhoto(HttpPostedFileBase Image, long UserId, string CoulmnName)
        {
            try
            {
                byte[] thePictureAsBytes = new byte[Image.ContentLength];
                using (BinaryReader theReader = new BinaryReader(Image.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(Image.ContentLength);
                }
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                string uri = "http://projectegy-001-site41.gtempurl.com/api/SetPhoto";
                var client = new HttpClient();
                var imageObject = new { RecordId = UserId, Table = "User", CoulmnName = CoulmnName, ImageType = (int)ImageFolders.User, Image = thePictureDataAsString };

                var myContent = JsonConvert.SerializeObject(imageObject);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, byteContent);
                var responseString = await response.Content.ReadAsStringAsync();
                var tempResponse = JObject.Parse(responseString);
                responseString = tempResponse.ToString();
                var responseCode = response.StatusCode;
                if (responseCode == HttpStatusCode.OK)
                {
                    var img = tempResponse.GetValue("model").ToString();
                }
            }
            catch (Exception ex)
            { }

        }
        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
            ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode", user.SponsorId);

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user, HttpPostedFileBase Image, HttpPostedFileBase IdentityFrontImage, HttpPostedFileBase IdentityBackImage)
        {
            if (ModelState.IsValid)
            {
                var OldUser = db.Users.AsNoTracking().Where(i => i.Id == user.Id).FirstOrDefault();
                if (OldUser.IdentityNumber!=user.IdentityNumber)
                {
                    var identityNumberFound = await GetUserByIdentityNumber(user.IdentityNumber);
                    if (identityNumberFound != null)
                    {
                        ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                        ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                        ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode", user.SponsorId);

                        ViewBag.Error = "رقم الهوية مسجل من قبل";
                        return View(user);

                    }
                }
                if (OldUser.PhoneNumber!=user.PhoneNumber)
                {
                    var phoneNumberFound = await GetUserByPhoneNumber(user.PhoneNumber);
                    if (phoneNumberFound != null)
                    {
                        ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                        ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                        ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode", user.SponsorId);

                        ViewBag.Error = "رقم الهاتف مسجل من قبل";
                        return View(user);

                    }

                }
                if (!user.IdentityNumber.StartsWith("2"))
                {
                    if (!user.IdentityNumber.StartsWith("3"))
                    {
                        ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                        ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                        ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode");

                        ViewBag.Error = "يجب ان يكون بداية الرقم القومي 2 او 3";
                        return View(user);

                    }
                }
                var DateOfBirth = user.DateOfBirth.ToShortDateString();
                var NationalId = user.IdentityNumber;
                var BDay = user.DateOfBirth.Day;
                var BMonth = user.DateOfBirth.Month;
                var BYear = Convert.ToInt32(user.DateOfBirth.Year.ToString().Substring(2, 2));
                var IYear = Convert.ToInt32(NationalId.Substring(1, 2));
                var IMonth = Convert.ToInt32(NationalId.Substring(3, 2));
                var IDay = Convert.ToInt32(NationalId.Substring(5, 2));
                if (BDay != IDay || BMonth != IMonth || IYear != BYear)
                {
                    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
                    ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode", user.SponsorId);

                    ViewBag.Error = "تاريخ الميلاد ورقم الهوية غير متطابقان ";
                    return View(user);

                }
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (Image != null)
                {
                    await SetPhoto(Image, user.Id, "ProfileImage");
                }
                if (IdentityFrontImage != null)
                {
                    await SetPhoto(IdentityFrontImage, user.Id, "IdentityFrontImage");
                }
                if (IdentityBackImage != null)
                {
                    await SetPhoto(IdentityBackImage, user.Id, "IdentityBackImage");

                }
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(OldUser.IdentityId);
                    var savingPassword = await UserManager.ResetPasswordAsync(OldUser.IdentityId, code, user.Password);
                }
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Name", user.AreaId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Name", user.GenderId);
            ViewBag.SponsorId = new SelectList(db.Users, "Id", "UserCode", user.SponsorId);

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> UsersReport()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
            return View(new List<UserReport>());
        }
        [HttpPost]
        public async Task<ActionResult> UsersReport(long UserId)
        {
            List<UserReport> userReports = new List<UserReport>();
            var sql = @"WITH UsersToRecursive AS
 (
SELECT Id,SponsorId,cast(0 as nvarchar(50)) as SponsorUserCode,FullName,UserCode,PhoneNumber,CreationDate FROM [User]  WHERE Id =" + UserId + "" +
@" UNION ALL
SELECT t.Id,t.SponsorId,c.UserCode as SponsorUserCode ,t.FullName,t.UserCode,t.PhoneNumber,t.CreationDate FROM [User] t INNER JOIN UsersToRecursive c  ON t.SponsorId = c.Id )
 SELECT *,(select IsNull(Sum(TotalPoints),0) from[Order] where UserId =UsersToRecursive.Id  and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as PersonalPoints FROM UsersToRecursive ";//where Id !=" + UserId + "

            var Users = await db.Database.SqlQuery<UserReport>(sql).ToListAsync();
            foreach (var user in Users)
            {
                user.GroupPoints = await GetGroupPoints(user.Id);
                var orderCount = db.Orders.Where(i => i.UserId == user.Id).Count();
                if (orderCount > 0)
                {
                    if (user.CreationDate != null)
                    {
                        if (user.CreationDate.Value.Month == DateTime.UtcNow.Month)
                        {
                            user.UserLable = "A";

                        }
                    }

                }
                else
                {
                    if (user.CreationDate != null)
                    {
                        if (user.CreationDate.Value.Month == DateTime.UtcNow.Month)
                        {
                            user.UserLable = "N";
                        }
                    }
                }

                user.GroupPoints = user.GroupPoints + user.PersonalPoints;
            }
            var userData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();
            if (Users.Count > 0)
            {
                userReports = Users;
                if (userData.SponsorId!=null)
                {
                    userReports.FirstOrDefault().SponsorUserCode = db.Users.Where(i => i.Id == userData.SponsorId).FirstOrDefault().UserCode;
                }
            }
            var Levels = db.LevelsSettings.ToList();

            foreach (var user in userReports)
            {
               

                user.UserLevel = "--";
                foreach (var item in Levels)
                {
                    if (user.GroupPoints >= item.LevelFrom && user.GroupPoints <= item.LevelTo)
                    {
                        user.UserLevel = item.LevelName;
                        break;
                    }

                }

            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode",UserId);
            ViewBag.UserData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();
            return View(userReports);
        }

        public async Task<int> GetGroupPoints(long UserId)
        {
            var sql = @"WITH UserTree
 AS
 (
    SELECT Id,SponsorId,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + "" +
 @" UNION ALL
    SELECT u.Id,u.SponsorId ,u.FullName,u.UserCode,u.PhoneNumber FROM [User] u 
	INNER JOIN UserTree c  ON u.SponsorId = c.Id
 ) 
select Isnull( Sum(PersonalPoints),0) as GroupPoints from (	SELECT *,(select IsNull( Sum(TotalPoints),0) from [Order] where UserId=UserTree.Id and 
(SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as PersonalPoints FROM UserTree 	where Id !=" + UserId + " ) as tbl where PersonalPoints<10000";
            var GroupPoints = await db.Database.SqlQuery<int>(sql).FirstOrDefaultAsync();
            return GroupPoints;
        }


        public async Task<ActionResult> UsersCompanyLevels()
        {
            List<UserReport> userReports = new List<UserReport>();
            var Users = await db.Database.SqlQuery<UserReport>("SELECT Id,SponsorId,FullName,UserCode,PhoneNumber,(select IsNull(Sum(TotalPoints),0) from[Order] where UserId =[User].Id and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as PersonalPoints FROM [User]  where SponsorId =31").ToListAsync();
            var Levels = db.LevelsSettings.ToList();

            foreach (var user in Users)
            {
                user.GroupPoints = await GetGroupPoints(user.Id);
                user.GroupPoints=user.PersonalPoints + user.GroupPoints;
                var UserPoints = user.GroupPoints;

                user.UserLevel = "--";
                foreach (var item in Levels)
                {
                    if (UserPoints >= item.LevelFrom && UserPoints <= item.LevelTo)
                    {
                        user.UserLevel = item.LevelName;
                        break;
                    }

                }
               
            }

            foreach (var levelData in Levels)
            {
               var count = Users.Where(i => i.UserLevel == levelData.LevelName).Count();
                levelData.Count = count;
                levelData.SumOfPoints = Users.Where(i => i.UserLevel == levelData.LevelName).Sum(i=>i.GroupPoints);

            }
            if (Users.Count > 0)
            {
                userReports = Users;
            }
            ViewBag.Levels = Levels.Where(i=>i.Count>0);
            ViewBag.LevelsCount = Levels.Where(i=>i.Count>0).Count();
            return View(userReports);
        }


        public JsonResult GetName(string searchTerm)
        {
            User user = new User() { Id=-1,UserCode="-1"};
            var categories = db.Users.Take(50).ToList();

            if (searchTerm != null)
            {
                categories = db.Users.Where(x => x.UserCode.Contains(searchTerm)).ToList();
            }
            categories.Add(user);
            categories = categories.OrderBy(i => i.Id).ToList();
            var modifiedData = categories.Select(x => new
            {
                id = x.Id,
                text = x.UserCode
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetUserPointsHistory(long? UserId)
        {
            if (UserId>0)
            {
                var Levels = db.LevelsSettings.ToList();

                string personalsql = @"Declare @UserId  bigint
set @UserId=" + UserId + " " +
@" select SUM(personalPoints) as PersonalPoints,ordermonth,orderYear from (
 select  IsNull( Sum(TotalPoints),0) as personalPoints,(SELECT DATEPART(MONTH, CreationDate)) as ordermonth,(SELECT DATEPART(YEAR, CreationDate)) as orderYear from [Order] where  [Order].UserId=@UserId
 group by  CreationDate) as outertbl
 where ordermonth>0
 group by ordermonth,orderYear ";
                var Personalpoints = db.Database.SqlQuery<UserPointsWithMonth>(personalsql).ToList();

                ///////////////////////////////////////////////////////////////////////////
                string Groupsql = @"WITH UserTree
 AS
 (
    SELECT Id,SponsorId,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + " " +
                @"  UNION ALL
    SELECT u.Id,u.SponsorId ,u.FullName,u.UserCode,u.PhoneNumber FROM [User] u 
	INNER JOIN UserTree c  ON u.SponsorId = c.Id
 ) 
 select Sum(grouppoint) as grouppoint ,ordermonth,orderYear from (
SELECT IsNull( Sum(TotalPoints),0) as grouppoint,(SELECT DATEPART(MONTH, CreationDate)) as ordermonth,(SELECT DATEPART(YEAR, CreationDate)) as orderYear
 FROM UserTree join [Order] on UserTree.Id=[Order].UserId  
  where UserTree.Id != =" + UserId + "  group by UserTree.Id,[Order].CreationDate) as outertbl where grouppoint<10000  group by ordermonth,orderYear ";
                var Grouppoints = db.Database.SqlQuery<UserPointsWithMonth>(personalsql).ToList();

                foreach (var item in Personalpoints)
                {
                    item.grouppoint = Grouppoints.Where(i => i.ordermonth == item.ordermonth && i.orderYear == item.orderYear).Sum(i => i.grouppoint);
                    item.grouppoint = item.grouppoint + item.PersonalPoints;

                    item.UserLevel = "--";
                    foreach (var level in Levels)
                    {
                        if (item.grouppoint >= level.LevelFrom && item.grouppoint <= level.LevelTo)
                        {
                            item.UserLevel = level.LevelName;
                            break;
                        }

                    }
                }

                
                ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode",UserId);
                ViewBag.UserData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();

                return View(Personalpoints);

            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");

            return View(new List<UserPointsWithMonth>());

        }

        [AllowAnonymous]
        public async Task<ActionResult> UsersReportForWebView(long UserId,long SponsorId=0)
        {
            if (SponsorId==0)
            {
                SponsorId = UserId;

            }
            ViewBag.SponsorId = SponsorId;
        var usersDrop=new SelectList(db.Users.Where(i => i.Id == UserId || i.SponsorId == SponsorId), "Id", "UserCode");
            ViewBag.UserId = usersDrop;
            List<UserReport> userReports = new List<UserReport>();
            var sql = @"WITH UsersToRecursive AS
 (
SELECT Id,SponsorId,cast(0 as nvarchar(50)) as SponsorUserCode,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + "" +
@" UNION ALL
SELECT t.Id,t.SponsorId,c.UserCode as SponsorUserCode ,t.FullName,t.UserCode,t.PhoneNumber FROM [User] t INNER JOIN UsersToRecursive c  ON t.SponsorId = c.Id )
 SELECT *,(select IsNull(Sum(TotalPoints),0) from[Order] where UserId =UsersToRecursive.Id  and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as PersonalPoints FROM UsersToRecursive where Id !=" + UserId + "";

            var Users = await db.Database.SqlQuery<UserReport>(sql).ToListAsync();
            foreach (var user in Users)
            {
                user.GroupPoints = await GetGroupPoints(user.Id);
            }
            if (Users.Count > 0)
            {
                userReports = Users;
            }
          

            return View(userReports);
        }

        [AllowAnonymous]
        public async Task<ActionResult> UserTree(long? UserId,long? LevelsSettingId)
        {
            //ViewBag.width = width;
            //ViewBag.height = height;
            
            var Levels = db.LevelsSettings.ToList();
           
            //ViewBag.LevelsSettingId = new SelectList(Levels, "Id", "LevelName");
            ViewBag.levels = Levels;
            ViewBag.UserId = UserId;
            var Favorites=db.Favorites.Where(x=>x.UserId==UserId).Select(x=>x.SelectedUserId).ToList();
            string levelName = "";
            if (LevelsSettingId>0)
            {
                levelName = Levels.Where(i => i.Id == LevelsSettingId).FirstOrDefault().LevelName;
            }
            string sql = @"WITH UsersToRecursive AS
 (
SELECT Id,ProfileImage,SponsorId,cast(0 as nvarchar(50)) as SponsorUserCode,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + " " +
@" UNION ALL
SELECT t.Id,t.ProfileImage,t.SponsorId,c.UserCode as SponsorUserCode ,t.FullName,t.UserCode,t.PhoneNumber FROM [User] t INNER JOIN UsersToRecursive c  ON t.SponsorId = c.Id )
SELECT Id,SponsorId,FullName as fname,UserCode as lname,(select cast(IsNull(Sum(TotalPoints),0) as nvarchar(50)) from[Order] where UserId =UsersToRecursive.Id  and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as title,ProfileImage as photo  FROM UsersToRecursive where Id !=" + UserId + "";

            var datalist = db.Database.SqlQuery<Parent>(sql).ToList();
            var userData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();
            Parent parent = new Parent()
            {
                fname = userData.FullName,
                lname = userData.UserCode,
                title = "",
                photo= "http://projectegy-001-site41.gtempurl.com/"+userData.ProfileImage,
                Id = userData.Id,
                
                
            };
            foreach (var item in datalist)
            {
                var GroupPoints = await GetGroupPoints(item.Id);
                var personal = Convert.ToInt32(item.title);
                var total = GroupPoints + personal;
                item.title = total.ToString();
                item.IsFavorite=Favorites.Contains(item.Id)?true:false;
                //item.ChildrenSum = GetUserPointsHistory(item.Id).ToString();
                if (string.IsNullOrEmpty(item.photo))
                {
                    item.photo = "http://projectegy-001-site41.gtempurl.com/images/userimages/115.png";
                }
                else { item.photo = "http://projectegy-001-site41.gtempurl.com/" + item.photo; }
                item.children = datalist.Where(i => i.SponsorId == item.Id).ToList();
                item.UserLevel = "--";
                foreach (var level in Levels)
                {
                    if (total >= level.LevelFrom && total <= level.LevelTo)
                    {
                        item.UserLevel = level.LevelName;
                        break;
                    }

                }
            }
            if (!string.IsNullOrEmpty( levelName))
            {
             datalist=   datalist.Where(i => i.UserLevel == levelName).ToList();
            }
            parent.children = datalist.Where(i => i.SponsorId == parent.Id).ToList();
           
            ViewBag.dd = parent;
            //ViewBag.Data = new JavaScriptSerializer().Serialize(parent);

            return View();
        }
        [AllowAnonymous]
        public   bool Favorite(long UserId,long Selected)
        {
            if (UserId<=0 || Selected<=0)
            {
                return false;
            }
            else
            {
                var Fav = db.Favorites.Where(x => x.UserId == UserId && x.SelectedUserId == Selected).FirstOrDefault();

                if (Fav !=null)
                {
                    db.Favorites.Remove(Fav);
                    db.SaveChanges();
                    return false;
                }
                else
                {
                    UserFavorite favorite = new UserFavorite()
                    {
                        CreationDate = DateTime.Now,
                        UserId = UserId,
                        SelectedUserId = Selected
                    };
                    db.Favorites.Add(favorite);
                    db.SaveChanges();
                    return true;

                }
               
            }
        }
        public UserReport PrintNodesRecursive(List<UserReport>  userReports, UserReport parent)
        {
            UserReport g = new UserReport();
            var users=  userReports.Where(i => i.SponsorId == parent.Id).ToList();
            parent.UserTrees = users;
            return parent;
        }

        [AllowAnonymous]
        public async Task<ActionResult> NewTree(long? UserId)
        {
//            var Levels = db.LevelsSettings.ToList();
//            ViewBag.LevelsSettingId = new SelectList(Levels, "Id", "LevelName");
//            ViewBag.UserId = UserId;
           
//            string sql = @"WITH UsersToRecursive AS
// (
//SELECT Id,ProfileImage,SponsorId,cast(0 as nvarchar(50)) as SponsorUserCode,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + " " +
//@" UNION ALL
//SELECT t.Id,t.ProfileImage,t.SponsorId,c.UserCode as SponsorUserCode ,t.FullName,t.UserCode,t.PhoneNumber FROM [User] t INNER JOIN UsersToRecursive c  ON t.SponsorId = c.Id )
//SELECT Id as id,SponsorId as pid,FullName as name,UserCode as code,(select cast(IsNull(Sum(TotalPoints),0) as nvarchar(50)) from[Order] where UserId =UsersToRecursive.Id  and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as personalPoints,ProfileImage as img   FROM UsersToRecursive where Id !=" + UserId + "";

//            var datalist = db.Database.SqlQuery<TreeModel>(sql).ToList();
//            var userData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();
//            List<string> tgs = new List<string>();
//            tgs.Add("root");
//            TreeModel parent = new TreeModel()
//            {
//                id = userData.Id,
//                pid= userData.Id,
//                personLevel="",
//                love="empty",
//                save=(int)userData.Id,
//                groupPoints="0",
//                personalPoints="0",
//                tags=tgs ,
//                name = userData.FullName,
//                code = userData.UserCode,
//                img = "http://projectegy-001-site41.gtempurl.com/" + userData.ProfileImage,
//                link = "http://projectegy-001-site41.gtempurl.com/Users/Details/" + userData.Id,
//            };
//            foreach (var item in datalist)
//            {
//                item.save = (int)item.pid;
//                item.tags = new List<string>();
//                item.love = "fill";
//                item.link = "http://projectegy-001-site41.gtempurl.com/Users/Details/" + item.id;
//                var GroupPoints = await GetGroupPoints(item.id);
//                var personal = Convert.ToInt32(item.personalPoints);
//                var total = GroupPoints + personal;
//                item.groupPoints = total.ToString();
//                if (string.IsNullOrEmpty(item.img))
//                {
//                    item.img = "http://projectegy-001-site41.gtempurl.com/images/userimages/115.png";
//                }
//                else { item.img = "http://projectegy-001-site41.gtempurl.com/" + item.img; }
//                item.personLevel = "--";
//                foreach (var level in Levels)
//                {
//                    if (total >= level.LevelFrom && total <= level.LevelTo)
//                    {
//                        item.personLevel = "18";//level.LevelName;
//                        break;
//                    }

//                }
//            }
//            datalist = datalist.Where(i => i.id != userData.Id).ToList();

//            datalist.Add(parent);
//            datalist = datalist.OrderBy(i => i.id).ToList();

//            ViewBag.Data = new JavaScriptSerializer().Serialize(parent);
//            JsonSerializer serializer = new JsonSerializer();
//            var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");

//            //serialize object directly into file stream
//            var file = basePath + "/treeNodes/test.json";
//            string json = JsonConvert.SerializeObject(datalist);
//            System.IO.File.WriteAllText(file, json);

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> UserTreev3(long? UserId, long? LevelsSettingId, int width = 4000, int height = 4000)
        {
            string result = "";

            ViewBag.width = width;
            ViewBag.height = height;

            var Levels = db.LevelsSettings.ToList();
            ViewBag.LevelsSettingId = new SelectList(Levels, "Id", "LevelName");
            ViewBag.levels = Levels;
            ViewBag.UserId = UserId;
            string levelName = "";
            if (LevelsSettingId > 0)
            {
                levelName = Levels.Where(i => i.Id == LevelsSettingId).FirstOrDefault().LevelName;
            }
            string sql = @"WITH UsersToRecursive AS
 (
SELECT Id,ProfileImage,SponsorId,cast(0 as nvarchar(50)) as SponsorUserCode,FullName,UserCode,PhoneNumber FROM [User]  WHERE Id =" + UserId + " " +
@" UNION ALL
SELECT t.Id,t.ProfileImage,t.SponsorId,c.UserCode as SponsorUserCode ,t.FullName,t.UserCode,t.PhoneNumber FROM [User] t INNER JOIN UsersToRecursive c  ON t.SponsorId = c.Id )
SELECT Id,SponsorId,FullName as fname,UserCode as lname,(select cast(IsNull(Sum(TotalPoints),0) as nvarchar(50)) from[Order] where UserId =UsersToRecursive.Id  and (SELECT DATEPART(Year, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate()))) and  ( (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) or (SELECT DATEPART(month, CancelationDate)) = (SELECT DATEPART(month, (select GetutcDate())))  )) as title,ProfileImage as photo  FROM UsersToRecursive where Id !=" + UserId + "";

            var datalist = db.Database.SqlQuery<Parent>(sql).ToList();
            var userData = db.Users.Where(i => i.Id == UserId).FirstOrDefault();
            Parent parent = new Parent()
            {
                fname = userData.FullName,
                lname = userData.UserCode,
                title = "",
                photo = "http://projectegy-001-site41.gtempurl.com/" + userData.ProfileImage,
                Id = userData.Id

            };
            string tbl = @" <ul>
                    <li>
                        <a href='javascript: void(0); '>
                              <div class='member-view-box'>
                                <div class='member-image'>
                                    <div class='imgg'>
                                        <img src = '~/treev3/img/v2-2.jpg' alt='Member'>
                                    </div>
                                    <span class='labeel backcircleredheart'>
                                        <i class='fas fa-heart  redheart'></i>
                                    </span>
                                    <span class='percentage'>" + parent.UserLevel + "</span>" +
                                    @"<span class='filter'>" + parent.children?.Count + "</span>" +
                                    @"<div class='member-details'>
                                    <p class='name'>" + parent.fname + "</p>" +
                                    @"<p class='code'>الكود " + parent.lname + "</p>" +
                                    @"<p class='points'> النقط الشخصية " + parent.title + "</p>" +
                                    @"<p class='team'>نقط المجموعة : " + parent.title + "</p>" +
                                    @"</div>
                                </div>
                            </div>
                        </a>
                        <ul class='active'>



 </ul>
                </li>
            </ul>
";

            List<string> temp = new List<string>();
            foreach (var item in datalist)
            {
                var GroupPoints = await GetGroupPoints(item.Id);
                var personal = Convert.ToInt32(item.title);
                var total = GroupPoints + personal;
                item.title = total.ToString();
                if (string.IsNullOrEmpty(item.photo))
                {
                    item.photo = "http://projectegy-001-site41.gtempurl.com/images/userimages/115.png";
                }
                else { item.photo = "http://projectegy-001-site41.gtempurl.com/" + item.photo; }
                item.children = datalist.Where(i => i.SponsorId == item.Id).ToList();
                item.UserLevel = "--";
                foreach (var level in Levels)
                {
                    if (total >= level.LevelFrom && total <= level.LevelTo)
                    {
                        item.UserLevel = level.LevelName;
                        break;
                    }

                }
                temp.Add(BuildTableNode(item));
            }
            if (!string.IsNullOrEmpty(levelName))
            {
                datalist = datalist.Where(i => i.UserLevel == levelName).ToList();
            }
            parent.children = datalist.Where(i => i.SponsorId == parent.Id).ToList();
            //ViewBag.Data = new JavaScriptSerializer().Serialize(parent);
            parent.children.FirstOrDefault().children = parent.children;
            ViewBag.Data =  parent;
            ViewBag.result = result;
            ViewBag.temp = temp;
            return View();
        }
        public string BuildTableNode(Parent parent)
        {
            StringBuilder table = new StringBuilder();
            table.Append(
                @"  <li>
                        <a href='javascript: void(0); '>
                              <div class='member-view-box'>
                                <div class='member-image'>
                                    <div class='imgg'>
                                        <img src = '~/treev3/img/v2-2.jpg' alt='Member'>
                                    </div>
                                    <span class='labeel backcircleredheart'>
                                        <i class='fas fa-heart  redheart'></i>
                                    </span>
                                    <span class='percentage'>" + parent.UserLevel + "</span>" +
                                    @"<span class='filter'>" + parent.children.Count + "</span>" +
                                    @"<div class='member-details'>
                                    <p class='name'>" + parent.fname + "</p>" +
                                    @"<p class='code'>الكود " + parent.lname + "</p>" +
                                    @"<p class='points'> النقط الشخصية " + parent.title + "</p>" +
                                    @"<p class='team'>نقط المجموعة : " + parent.title + "</p>" +
                                    @"</div>
                                </div>
                            </div>
                        </a>
                        
                "
                );
            if (parent.children.Count>0)
            {
                table.Append(@"<ul>");

            }
            foreach (var item in parent.children)
            {
                 
                table.Append(
                     @"<li>
                           <a href ='javascript:void(0);'>
                           <div class='member-view-box'>
                           <div class='member-image'>
                           <div class='imgg'>
                           <img src ='~/treev3/img/v2-2.jpg' alt='Member'>
                           </div>
                           <span class='labeel backcircleredheart'>
                           <i class='fas fa-heart  redheart'></i>
                           </span>
                           <span class='percentage'>"+item.UserLevel+"</span> "+
                         @" <div class='member-details'>
                             <p class='name'>" + item.fname+"</p>"+
                             @"<p class='code'>الكود "+item.lname+"</p>"+
                             @"<p class='points'> النقط الشخصية "+item.title+"</p>"+
                             @"<p class='team'>نقط المجموعة : "+item.title+"</p>"+
                         @"</div>
                     </div>
                 </div>
                  </a>
                </li>
              "
                    );

            }
            if (parent.children.Count > 0)
            {
                table.Append(@"</ul>");

            }

            table.Append("</li>");

            return table.ToString();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
