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
    public class UserChequeNotesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: UserChequeNotes
        public async Task<ActionResult> Index()
        {
            return View(await db.UserChequeNotes.ToListAsync());
        }

        // GET: UserChequeNotes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserChequeNote userChequeNote = await db.UserChequeNotes.FindAsync(id);
            if (userChequeNote == null)
            {
                return HttpNotFound();
            }
            return View(userChequeNote);
        }

        // GET: UserChequeNotes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserChequeNotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,ChequeId,ChequeNumber,Status,CreationDate,Note")] UserChequeNote userChequeNote)
        {
            if (ModelState.IsValid)
            {
                db.UserChequeNotes.Add(userChequeNote);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(userChequeNote);
        }

        // GET: UserChequeNotes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserChequeNote userChequeNote = await db.UserChequeNotes.FindAsync(id);
            if (userChequeNote == null)
            {
                return HttpNotFound();
            }
            return View(userChequeNote);
        }

        // POST: UserChequeNotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,ChequeId,ChequeNumber,Status,CreationDate,Note")] UserChequeNote userChequeNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userChequeNote).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userChequeNote);
        }

        // GET: UserChequeNotes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserChequeNote userChequeNote = await db.UserChequeNotes.FindAsync(id);
            if (userChequeNote == null)
            {
                return HttpNotFound();
            }
            return View(userChequeNote);
        }

        // POST: UserChequeNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            UserChequeNote userChequeNote = await db.UserChequeNotes.FindAsync(id);
            db.UserChequeNotes.Remove(userChequeNote);
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
