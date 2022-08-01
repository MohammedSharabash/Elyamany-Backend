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
using System.Data.Entity.Migrations;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class UserChequesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: UserCheques
        public async Task<ActionResult> Index()
        {
            var userCheques = db.UserCheques.Include(u => u.User);
            return View(await userCheques.ToListAsync());
        }

        public async Task<ActionResult> GetCheques()
        {
            var sql = @" select *,(select count(Id) from [User] where ChequeNumber=Cheque.Number) as UserCount 
,(select Count(UserId) from (select  UserId,Count(ChequeNumber) UserCount from UserCheque where ChequeNumber=Cheque.Number
group by UserId) as tbl where UserCount>=8) as UsersTarget from Cheque";
            var cheques = db.Database.SqlQuery<ChequeView>(sql);
            return View(cheques);
        }

        public async Task<ActionResult> GetUserChequeViewForTarget()
        {

         var cheques=    db.UserChequeViewForTargets.ToList();
            return View(cheques);
        }


        public ActionResult EditUserChequeNotes(long Id,int ChequeNumber)
        {
            var cheques = db.UserChequeViewForTargets.Where(i=>i.Id==Id&&i.ChequeNumber==ChequeNumber).FirstOrDefault();

            if(cheques.UserChequeNote!=null)
            {
                cheques.Status = cheques.UserChequeNote.Status;
                cheques.Note = cheques.UserChequeNote.Note;
                cheques.UserChequeNoteId = cheques.UserChequeNote.Id;
            }
            return View(cheques);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserChequeNotes(UserChequeViewForTarget  userChequeViewForTarget)
        {
            if (ModelState.IsValid)
            {
                if (userChequeViewForTarget.UserChequeNoteId>0)
                {
                    var oldData = db.UserChequeNotes.AsNoTracking().Where(i => i.Id == userChequeViewForTarget.UserChequeNoteId).FirstOrDefault();
                    oldData.Note = userChequeViewForTarget.Note;
                    oldData.Status = userChequeViewForTarget.Status;
                    db.UserChequeNotes.AddOrUpdate(oldData);
                    await db.SaveChangesAsync();

                }
                else
                {
                    UserChequeNote userChequeNote = new UserChequeNote()
                    {
                        ChequeNumber = userChequeViewForTarget.ChequeNumber,
                        CreationDate = DateTime.UtcNow.AddHours(2),
                        Note = userChequeViewForTarget.Note,
                        Status = userChequeViewForTarget.Status,
                        UserId = userChequeViewForTarget.Id,
                        ChequeId = 0

                    };
                    db.UserChequeNotes.Add(userChequeNote);
                    await db.SaveChangesAsync();

                }
                return RedirectToAction("GetUserChequeViewForTarget");
            }

            return View("EditUserChequeNotes",new { Id= userChequeViewForTarget.Id, ChequeNumber = userChequeViewForTarget .ChequeNumber});
        }


        public async Task<ActionResult> GetChequeReport()
        {
            List<UserChequeView> userChequeViews = new List<UserChequeView>();
            var sql = @"select Id, FullName,UserCode,PhoneNumber,(select count(UserCheque.Id) from UserCheque where UserId=[User].Id and ChequeNumber=[User].ChequeNumber) as ChequeCount ,
(select top 1 Cheque.Amount from Cheque where Number=[User].ChequeNumber) as ChequeAmount
 from [User] ";
            var reports = await db.Database.SqlQuery<UserChequeView>(sql).ToListAsync();
            if (reports.Count>0)
            {
                userChequeViews = reports;
            }
            return View(userChequeViews);
        }
        // GET: UserCheques/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheque userCheque = await db.UserCheques.FindAsync(id);
            if (userCheque == null)
            {
                return HttpNotFound();
            }
            return View(userCheque);
        }

        // GET: UserCheques/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
            return View();
        }

        // POST: UserCheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserId,CreationDate")] UserCheque userCheque)
        {
            if (ModelState.IsValid)
            {
                db.UserCheques.Add(userCheque);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userCheque.UserId);
            return View(userCheque);
        }

        // GET: UserCheques/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheque userCheque = await db.UserCheques.FindAsync(id);
            if (userCheque == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userCheque.UserId);
            return View(userCheque);
        }

        // POST: UserCheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserId,CreationDate")] UserCheque userCheque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userCheque).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", userCheque.UserId);
            return View(userCheque);
        }

        // GET: UserCheques/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheque userCheque = await db.UserCheques.FindAsync(id);
            if (userCheque == null)
            {
                return HttpNotFound();
            }
            return View(userCheque);
        }

        // POST: UserCheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            UserCheque userCheque = await db.UserCheques.FindAsync(id);
            db.UserCheques.Remove(userCheque);
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
