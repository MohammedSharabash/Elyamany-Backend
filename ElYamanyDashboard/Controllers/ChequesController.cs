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
using System.IO;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class ChequesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: Cheques
        public async Task<ActionResult> Index()
        {
            return View(await db.Cheques.ToListAsync());
        }

        // GET: Cheques/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cheque cheque = await db.Cheques.FindAsync(id);
            if (cheque == null)
            {
                return HttpNotFound();
            }
            return View(cheque);
        }

        // GET: Cheques/Create
        public ActionResult Create()
        {
            return View();
        }

        public string SetPhoto(HttpPostedFileBase ImageFile)
        {
            string imagePath = null;
            try
            {
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                imagePath = "/Images/SymbolImages/";
                string fileName = "UI" + DateTime.Now.ToString("IMG" + "dd_MM_yyyy_HH_mm_ss") + ".png";
                byte[] thePictureAsBytes = new byte[ImageFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(ImageFile.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(ImageFile.ContentLength);
                }
                MemoryStream ms = new MemoryStream(thePictureAsBytes);
                string fullPath = basePath + imagePath + fileName;
                System.IO.Directory.CreateDirectory(basePath + imagePath);
                FileStream fs = new FileStream(fullPath, FileMode.Create);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();

                return fileName;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        // POST: Cheques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Cheque cheque, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    cheque.Symbol = SetPhoto(ImageFile);
                }
                db.Cheques.Add(cheque);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(cheque);
        }

        // GET: Cheques/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cheque cheque = await db.Cheques.FindAsync(id);
            if (cheque == null)
            {
                return HttpNotFound();
            }
            return View(cheque);
        }

        // POST: Cheques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Cheque cheque, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    cheque.Symbol = SetPhoto(ImageFile);
                }
                db.Entry(cheque).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(cheque);
        }

        // GET: Cheques/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cheque cheque = await db.Cheques.FindAsync(id);
            if (cheque == null)
            {
                return HttpNotFound();
            }
            return View(cheque);
        }

        // POST: Cheques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Cheque cheque = await db.Cheques.FindAsync(id);
            db.Cheques.Remove(cheque);
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
