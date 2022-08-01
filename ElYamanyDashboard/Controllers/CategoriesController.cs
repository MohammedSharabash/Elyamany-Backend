using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ElYamanyDashboard.Models;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class CategoriesController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    category.Image = SetPhoto(ImageFile);
                }
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    category.Image = SetPhoto(ImageFile);
                }
                db.Entry(category).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        // GET: Categories/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string SetPhoto(HttpPostedFileBase ImageFile)
        {
            string imagePath = null;
            try
            {
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                imagePath = "/Images/CategoryImages/";
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

        public JsonResult GetName(string searchTerm)
        {
            var categories = db.Categories.ToList();

            if (searchTerm != null)
            {
                categories = db.Categories.Where(x => x.NameAR.Contains(searchTerm)).ToList();
            }

            var modifiedData = categories.Select(x => new
            {
                id = x.Id,
                text = x.NameAR
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);
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
