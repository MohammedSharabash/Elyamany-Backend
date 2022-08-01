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
using System.Net.Http;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: Settings
        public async Task<ActionResult> Index(string Message="")
        {
            ViewBag.Message = Message;
            return View(await db.Settings.ToListAsync());
        }


        public async Task<ActionResult> CalculateSalaryForAllUsers()
        {
            var task = Task.Run(async () =>
            {
                var enpointUrl = "http://projectegy-001-site41.gtempurl.com/api/CalculateSalaryForAllUsers";
                var client = new HttpClient();
                var response = await client.GetAsync(enpointUrl);
                var responseString = await response.Content.ReadAsStringAsync();
            });

            return RedirectToAction("Index",new {Message="يتم الأن حساب القبض لكل الاعضاء الرجاء مراجعة صفحة القبض بعد عدة دقائق "});

        }

        public async Task<ActionResult> StopMakingOrder()
        {

            var settting = db.Settings.FirstOrDefault();
            settting.MakeOrders = false;
            await db.SaveChangesAsync();
            return RedirectToAction("Index",new {Message="تم ايقاف الطلبات للأعضاء"});

        }
        public async Task<ActionResult> EnableMakingOrder()
        {

            var settting = db.Settings.FirstOrDefault();
            settting.MakeOrders = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { Message = "تم تفعيل الطلبات للأعضاء" });

        }
        // GET: Settings/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting setting = await db.Settings.FindAsync(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // GET: Settings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Settings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MakeOrders,Whatsapp")] Setting setting)
        {
            if (ModelState.IsValid)
            {
                db.Settings.Add(setting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(setting);
        }

        // GET: Settings/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting setting = await db.Settings.FindAsync(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MakeOrders,Whatsapp")] Setting setting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(setting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(setting);
        }

        // GET: Settings/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting setting = await db.Settings.FindAsync(id);
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        // POST: Settings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Setting setting = await db.Settings.FindAsync(id);
            db.Settings.Remove(setting);
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
