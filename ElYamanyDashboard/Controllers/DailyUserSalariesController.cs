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

namespace ElYamanyDashboard.Controllers
{
    [Authorize]
    public class DailyUserSalariesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: DailyUserSalaries
        public async Task<ActionResult> Index()
        {

         var   userSalaries = await db.DailyUserSalaries.Include(u => u.User).OrderByDescending(i=>i.Id).ToListAsync();

        
            ViewBag.Total = userSalaries.Count;
 
            return View(userSalaries);
        }

        // GET: DailyUserSalaries/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyUserSalary dailyUserSalary = await db.DailyUserSalaries.FindAsync(id);
            if (dailyUserSalary == null)
            {
                return HttpNotFound();
            }
            return View(dailyUserSalary);
        }

        // GET: DailyUserSalaries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DailyUserSalaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,PersonalPoints,GroupPoints,Deduction,SalaryAmount,CreationDate,PayDate,Status")] DailyUserSalary dailyUserSalary)
        {
            if (ModelState.IsValid)
            {
                db.DailyUserSalaries.Add(dailyUserSalary);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dailyUserSalary);
        }

        // GET: DailyUserSalaries/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyUserSalary dailyUserSalary = await db.DailyUserSalaries.FindAsync(id);
            if (dailyUserSalary == null)
            {
                return HttpNotFound();
            }
            return View(dailyUserSalary);
        }

        // POST: DailyUserSalaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,PersonalPoints,GroupPoints,Deduction,SalaryAmount,CreationDate,PayDate,Status")] DailyUserSalary dailyUserSalary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyUserSalary).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dailyUserSalary);
        }

        // GET: DailyUserSalaries/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyUserSalary dailyUserSalary = await db.DailyUserSalaries.FindAsync(id);
            if (dailyUserSalary == null)
            {
                return HttpNotFound();
            }
            return View(dailyUserSalary);
        }

        // POST: DailyUserSalaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            DailyUserSalary dailyUserSalary = await db.DailyUserSalaries.FindAsync(id);
            db.DailyUserSalaries.Remove(dailyUserSalary);
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
