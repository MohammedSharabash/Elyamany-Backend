using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElYamanyDashboard.Models;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class ContactUsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: ContactUs
        public ActionResult Index(int IsReviewed=0)
        {
            List<ContactUs> contactUs = new List<ContactUs>();
            if (IsReviewed==1)
            {
                contactUs = db.ContactUs.Include(c => c.User).Where(i=>i.IsReviewed==true).ToList();
            }
            else if (IsReviewed == 2)
            {
                contactUs = db.ContactUs.Include(c => c.User).Where(i => i.IsReviewed == false).ToList();
            }
            else
            {
                contactUs = db.ContactUs.Include(c => c.User).ToList();

            }
            ViewBag.Total = contactUs.Count;
            ViewBag.IndexName = "Index";
            ViewBag.ColorAll = "#519839";

            return View(contactUs.OrderByDescending(i=>i.Id));
        }

        public ActionResult Index2()
        {
            List<ContactUs> contactUs = new List<ContactUs>();
           
           contactUs = db.ContactUs.Include(c => c.User).Where(i => i.IsReviewed == false).ToList();
            
            ViewBag.Total = contactUs.Count;
            ViewBag.IndexName = "Index2";
            ViewBag.Color2 = "#519839";

            return View(contactUs.OrderByDescending(i => i.Id));
        }

        public ActionResult Index3()
        {
            List<ContactUs> contactUs = new List<ContactUs>();

            contactUs = db.ContactUs.Include(c => c.User).Where(i => i.IsReviewed == true).ToList();

            ViewBag.Total = contactUs.Count;
            ViewBag.IndexName = "Index3";
            ViewBag.Color3 = "#519839";
            return View(contactUs.OrderByDescending(i => i.Id));
        }

        // GET: ContactUs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // GET: ContactUs/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName");
            return View();
        }

        // POST: ContactUs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Message,CreationDate")] ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                db.ContactUs.Add(contactUs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", contactUs.UserId);
            return View(contactUs);
        }

        // GET: ContactUs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", contactUs.UserId);
            return View(contactUs);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( ContactUs contactUs)
        {
            if (contactUs.UserId>0)
            {
                //db.Entry(contactUs).State = EntityState.Modified;
                db.ContactUs.AddOrUpdate(contactUs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", contactUs.UserId);
            return View(contactUs);
        }
        public ActionResult MarkItIsReviewed(long Id,string IndexName="index")
        {
            ContactUs contactUs = db.ContactUs.Find(Id);

            contactUs.IsReviewed = true;
            db.ContactUs.AddOrUpdate(contactUs);
            db.SaveChanges();
            return RedirectToAction(IndexName);

        }
        // GET: ContactUs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ContactUs contactUs = db.ContactUs.Find(id);
            db.ContactUs.Remove(contactUs);
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
