using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ElYamanyDashboard.Models;
using ElYamanyDashboard.Models.Views;
using ElYamanyDashboard.Utils;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class NotificationsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: Notifications
        public ActionResult Index(long? UserId)
        {
            List<Notification> notifications = new List<Notification>();
            if (UserId>0)
            {
                 notifications = db.Notifications.Include(n => n.NotificationType).Include(n => n.User).Where(i => i.UserId == UserId).OrderByDescending(i=>i.Id).ToList();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode",UserId);

            return View(notifications);

        }

        public ActionResult IndexLevels( )
        {
           
             //var notifications = db.Notifications.Where(i => i.UserLevel.Length>=1).OrderByDescending(i => i.Id).ToList();
            var notifications = db.Database.SqlQuery<NotificationView>("select distinct Message,MessageAR,Cast (CreationDate as date) as CreationDate,UserLevel from Notification where len(UserLevel)>=1").ToList();

            return View(notifications);

        }
        public ActionResult HelpMemeberIndex()
        {
            List<Notification> notifications = new List<Notification>();
            notifications = db.Notifications.Include(n => n.NotificationType).Include(n => n.User).Where(i => i.NotificationTypeId == 2).OrderByDescending(i => i.Id).ToList();
            return View(notifications);

        }


        public ActionResult ItemsNotifications()
        {
            List<Notification> notifications = new List<Notification>();
            notifications = db.Notifications.Include(n => n.NotificationType).Where(i => i.NotificationTypeId == 4).OrderByDescending(i => i.Id).ToList();
            return View(notifications);

        }

        // GET: Notifications/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create(string title)
        {
            ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes.Where(i=>i.Id==1||i.Id==3), "Id", "NameAr");
            ViewBag.LevelSettingId = new SelectList(db.LevelsSettings, "Id", "LevelName");
            ViewBag.Message = title;
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
            return View(new Notification());
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create( Notification notification)
        {
            notification.IsSeen = false;
            if (ModelState.IsValid)
            {
                List<PushToken> IosTokens = new List<PushToken>();
                List<PushToken> AndroidTokens = new List<PushToken>();
                if (notification.ForAll==true)
                {
                    IosTokens = db.PushTokens.Where(i => i.OS.ToLower() == "ios").ToList();
                    AndroidTokens = db.PushTokens.Where(i => i.OS.ToLower() == "android").ToList();
                    foreach (var item in IosTokens)
                    {

                    }
                    foreach (var item in AndroidTokens)
                    {
                        Notification notification2 = new Notification()
                        {
                            NotificationToId = 0,
                            CreationDate = DateTime.UtcNow,
                            Message = notification.Message,
                            MessageAR = notification.MessageAR,
                            NotificationTypeId = notification.NotificationTypeId,
                            UserLevel = "",
                            UserId = item.UserId,
                            IsSeen=false
                        };
                        db.Notifications.Add(notification2);
                        db.SaveChanges();

                        PushManager.pushToAndroidDevice(item.Token, "Elyamany", notification.MessageAR, notification.NotificationTypeId.Value, 0);
                    }

                    ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes.Where(i => i.Id == 1 || i.Id == 3), "Id", "NameAr", notification.NotificationTypeId);
                    ViewBag.LevelSettingId = new SelectList(db.LevelsSettings, "Id", "LevelName");
                    ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
                    ViewBag.Message = "لقد تم ارسال الاشعار بنجاح";
                    return RedirectToAction("Create", new { title = "لقد تم ارسال الاشعار بنجاح" });

                }
                else if (notification.UserId>0)
                {
                    notification.CreationDate = DateTime.UtcNow.AddHours(2);
                    notification.NotificationToId = 0;
                    notification.IsSeen = false;
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                     IosTokens     = db.PushTokens.Where(i => i.UserId == notification.UserId.Value && i.OS.ToLower() == "ios").ToList();
                     AndroidTokens = db.PushTokens.Where(i => i.UserId == notification.UserId.Value && i.OS.ToLower() == "android").ToList();


                    foreach (var item in IosTokens)
                    {

                    }
                    foreach (var item in AndroidTokens)
                    {
                        PushManager.pushToAndroidDevice(item.Token, "Elyamany", notification.MessageAR, notification.NotificationTypeId.Value, 0);
                    }

                    ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes.Where(i => i.Id == 1 || i.Id == 3), "Id", "NameAr", notification.NotificationTypeId);
                    ViewBag.LevelSettingId = new SelectList(db.LevelsSettings, "Id", "LevelName");
                    ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
                    ViewBag.Message = "لقد تم ارسال الاشعار بنجاح";

                    
                    return RedirectToAction("Create",new { title = "لقد تم ارسال الاشعار بنجاح" });

                }
                else
                {
                    var users =await GetUsersWithLevels();
                    users = users.Where(i => i.LevelId == notification.LevelSettingId).ToList();
                    var IosTokens2 = users.Where(i => i.IOSToken != null).ToList();
                    var AndroidTokens2 = users.Where(i => i.AndroidToken != null).ToList();
                    foreach (var item in IosTokens2)
                    {

                    }
                    foreach (var item in AndroidTokens2)
                    {
                        Notification notification2 = new Notification()
                        {
                            NotificationToId = 0,
                            CreationDate=DateTime.UtcNow,
                            Message=notification.Message,
                            MessageAR=notification.MessageAR,
                            NotificationTypeId=notification.NotificationTypeId,
                            UserLevel = item.UserLevel,
                            UserId=item.Id,
                            IsSeen=false
                        };
                        db.Notifications.Add(notification2);
                        db.SaveChanges();

                        PushManager.pushToAndroidDevice(item.AndroidToken, "Elyamany", notification.MessageAR, notification.NotificationTypeId.Value, 0);
                    }

                    ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes.Where(i => i.Id == 1 || i.Id == 3), "Id", "NameAr", notification.NotificationTypeId);
                    ViewBag.LevelSettingId = new SelectList(db.LevelsSettings, "Id", "LevelName");
                    ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
                    ViewBag.Message = "لقد تم ارسال الاشعار بنجاح";
                    return RedirectToAction("Create", new { title = "لقد تم ارسال الاشعار بنجاح" });


                }


            }
            ViewBag.LevelSettingId = new SelectList(db.LevelsSettings, "Id", "LevelName");

            ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes, "Id", "NameAr", notification.NotificationTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", notification.UserId);
            return View(notification);
        }


        public async Task<List<UserReport>> GetUsersWithLevels()
        {
            List<UserReport> userReports = new List<UserReport>();

            var Levels = db.LevelsSettings.ToList();

            string sql = @" SELECT *,(select top 1 Token from PushToken where UserId=[User].Id and OS='ios') as IOSToken,(select top 1 Token from PushToken where UserId=[User].Id and OS='android') as AndroidToken,(select IsNull(Sum(TotalPoints),0) from[Order] where UserId =[User].Id  and (SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate()))) and (SELECT DATEPART(YEAR, CreationDate)) = (SELECT DATEPART(YEAR, (select GetutcDate())))) as PersonalPoints FROM [User] ";
            var Users = await db.Database.SqlQuery<UserReport>(sql).ToListAsync();
            foreach (var user in Users)
            {
              await  GetGroupPoints(user.Id);
                user.GroupPoints = user.PersonalPoints + user.GroupPoints;
                var UserPoints = user.GroupPoints;
                foreach (var item in Levels)
                {
                    if (UserPoints >= item.LevelFrom && UserPoints <= item.LevelTo)
                    {
                        user.UserLevel = item.LevelName;
                        user.LevelId = item.Id;
                        break;
                    }
                }
            }
            return Users;
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
select Isnull( Sum(PersonalPoints),0) as GroupPoints from (	SELECT *,(select IsNull( Sum(TotalPoints),0) from [Order] where UserId=UserTree.Id and (SELECT DATEPART(month, CreationDate))=(SELECT DATEPART(month, (select GetutcDate())))
and (SELECT DATEPART(YEAR, CreationDate))=(SELECT DATEPART(YEAR, (select GetutcDate())))
) as PersonalPoints FROM UserTree 	where Id !=" + UserId + " ) as tbl where PersonalPoints<10000";
            var GroupPoints = await db.Database.SqlQuery<int>(sql).FirstOrDefaultAsync();
            return GroupPoints;
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes, "Id", "Name", notification.NotificationTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", notification.UserId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,NotificationTypeId,Message,MessageAR,CreationDate")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NotificationTypeId = new SelectList(db.NotificationTypes, "Id", "Name", notification.NotificationTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", notification.UserId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
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
