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

    public class LevelsSettingsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: LevelsSettings
        public async Task<ActionResult> Index()
        {
            return View(await db.LevelsSettings.ToListAsync());
        }

        // GET: LevelsSettings/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelsSetting levelsSetting = await db.LevelsSettings.FindAsync(id);
            if (levelsSetting == null)
            {
                return HttpNotFound();
            }
            return View(levelsSetting);
        }

        // GET: LevelsSettings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LevelsSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LevelsSetting levelsSetting)
        {
            if (ModelState.IsValid)
            {
                db.LevelsSettings.Add(levelsSetting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(levelsSetting);
        }

        // GET: LevelsSettings/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelsSetting levelsSetting = await db.LevelsSettings.FindAsync(id);
            if (levelsSetting == null)
            {
                return HttpNotFound();
            }
            return View(levelsSetting);
        }

        // POST: LevelsSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LevelsSetting levelsSetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(levelsSetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(levelsSetting);
        }

        // GET: LevelsSettings/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LevelsSetting levelsSetting = await db.LevelsSettings.FindAsync(id);
            if (levelsSetting == null)
            {
                return HttpNotFound();
            }
            return View(levelsSetting);
        }

        // POST: LevelsSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            LevelsSetting levelsSetting = await db.LevelsSettings.FindAsync(id);
            db.LevelsSettings.Remove(levelsSetting);
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
