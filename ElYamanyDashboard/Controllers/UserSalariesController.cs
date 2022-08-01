using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElYamanyDashboard.Models;
using ElYamanyDashboard.Models.Views;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class UserSalariesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: UserSalaries
        public async Task<ActionResult> Index(int IsDeliverd=0)
        {
            List<UserSalary> userSalaries = new List<UserSalary>();
            ViewBag.TotalSalaryInThisMonth = db.Database.SqlQuery<decimal>(@" SELECT isnull( Sum(SalaryAmount-Deduction) ,0)as TotalSalary from UserSalary
 where(SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month,(select GetutcDate())))").FirstOrDefault();
           
            ViewBag.TotalSalaryInPastMonth = db.Database.SqlQuery<decimal>(@" SELECT isnull( Sum(SalaryAmount-Deduction) ,0)as TotalSalary from UserSalary
 where(SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month,(select GetutcDate())))-1").FirstOrDefault();

            if (IsDeliverd==1)
            {
                userSalaries = await db.UserSalaries.Include(u => u.User).Where(i=>i.Status==true&& i.CreationDate.Value.Month >= DateTime.UtcNow.Month - 1).ToListAsync();

            }
            if (IsDeliverd == 2)
            {
                userSalaries = await db.UserSalaries.Include(u => u.User).Where(i => i.Status == false&&i.CreationDate.Value.Month >= DateTime.UtcNow.Month - 1).ToListAsync();

            }
            else
            {

                userSalaries = await db.UserSalaries.Include(u => u.User).Where(o=>o.CreationDate.Value.Month>=DateTime.UtcNow.Month-1).ToListAsync();

            }
            ViewBag.TotalPastMonth = userSalaries.Where(o => o.CreationDate.Value.Month == DateTime.UtcNow.Month -1).Count();
            ViewBag.TotalCurrentMonth = userSalaries.Where(o => o.CreationDate.Value.Month == DateTime.UtcNow.Month).Count();
            return View(userSalaries.OrderByDescending(i=>i.SalaryAmount));
        }




        public async Task<ActionResult> IndexForAll(int IsDeliverd = 0)
        {
            List<UserSalary> userSalaries = new List<UserSalary>();
 //           ViewBag.TotalSalaryInThisMonth = db.Database.SqlQuery<decimal>(@" SELECT isnull( Sum(SalaryAmount-Deduction) ,0)as TotalSalary from UserSalary
 //where(SELECT DATEPART(month, CreationDate)) = (SELECT DATEPART(month, (select GetutcDate())))").FirstOrDefault();
            if (IsDeliverd == 1)
            {
                userSalaries = await db.UserSalaries.Include(u => u.User).Where(i => i.Status == true).ToListAsync();

            }
            if (IsDeliverd == 2)
            {
                userSalaries = await db.UserSalaries.Include(u => u.User).Where(i => i.Status == false).ToListAsync();

            }
            else
            {

                userSalaries = await db.UserSalaries.Include(u => u.User).ToListAsync();

            }
            ViewBag.Total = userSalaries.Count;
            return View(userSalaries);
        }

        // GET: UserSalaries/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSalary userSalary = await db.UserSalaries.FindAsync(id);
            if (userSalary == null)
            {
                return HttpNotFound();
            }
            return View(userSalary);
        }

        // GET: UserSalaries/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
            return View();
        }

        // POST: UserSalaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,PersonalPoints,GroupPoints,Deduction,SalaryAmount,CreationDate,Status")] UserSalary userSalary)
        {
            if (ModelState.IsValid)
            {
                db.UserSalaries.Add(userSalary);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userSalary.UserId);
            return View(userSalary);
        }

        // GET: UserSalaries/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSalary userSalary = await db.UserSalaries.FindAsync(id);
            if (userSalary == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userSalary.UserId);
            return View(userSalary);
        }

        // POST: UserSalaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserSalary userSalary)
        {
            if (ModelState.IsValid)
            {
                var oldsalary = db.UserSalaries.Where(i => i.Id == userSalary.Id).FirstOrDefault();
                if (oldsalary.Status==false&&userSalary.Status==true)
                {
                    userSalary.PayDate = DateTime.UtcNow;
                }
                db.Entry(userSalary).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userSalary.UserId);
            return View(userSalary);
        }

        // GET: UserSalaries/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSalary userSalary = await db.UserSalaries.FindAsync(id);
            if (userSalary == null)
            {
                return HttpNotFound();
            }
            return View(userSalary);
        }

        // POST: UserSalaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            UserSalary userSalary = await db.UserSalaries.FindAsync(id);
            db.UserSalaries.Remove(userSalary);
            await db.SaveChangesAsync();
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
