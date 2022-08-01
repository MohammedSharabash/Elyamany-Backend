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
    public class DeliveryTypesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: DeliveryTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.DeliveryTypes.ToListAsync());
        }

        // GET: DeliveryTypes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryType deliveryType = await db.DeliveryTypes.FindAsync(id);
            if (deliveryType == null)
            {
                return HttpNotFound();
            }
            return View(deliveryType);
        }

        // GET: DeliveryTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeliveryTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,NameAR,Cost")] DeliveryType deliveryType)
        {
            if (ModelState.IsValid)
            {
                db.DeliveryTypes.Add(deliveryType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(deliveryType);
        }

        // GET: DeliveryTypes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryType deliveryType = await db.DeliveryTypes.FindAsync(id);
            if (deliveryType == null)
            {
                return HttpNotFound();
            }
            return View(deliveryType);
        }

        // POST: DeliveryTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,NameAR,Cost")] DeliveryType deliveryType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(deliveryType);
        }

        // GET: DeliveryTypes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryType deliveryType = await db.DeliveryTypes.FindAsync(id);
            if (deliveryType == null)
            {
                return HttpNotFound();
            }
            return View(deliveryType);
        }

        // POST: DeliveryTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            DeliveryType deliveryType = await db.DeliveryTypes.FindAsync(id);
            db.DeliveryTypes.Remove(deliveryType);
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
