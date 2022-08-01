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
    public class ProductsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();

        // GET: Products
        public ActionResult Index(long? CategoryId)
        {
            if (CategoryId>0)
            {
                var productsForCategory = db.ProductViews.Include(p => p.Category).Where(i=>i.CategoryId==CategoryId).ToList();
                return View(productsForCategory);

            }
            var products = db.ProductViews.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "NameAR");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    product.Image = SetPhoto(ImageFile);
                }
                db.Products.Add(product);
               await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "NameAR", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "NameAR", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {

                if (ImageFile != null)
                {
                    product.Image = SetPhoto(ImageFile);
                }
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "NameAR", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string SetPhoto(HttpPostedFileBase ImageFile)
        {
            string imagePath = null;
            try
            {
                var basePath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                imagePath = "/Images/ProductImages/";
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
            var products = db.Products.Take(50).ToList();

            if (searchTerm != null)
            {
                products = db.Products.Where(x => x.NameAR.Contains(searchTerm)).ToList();
            }
             products = products.OrderBy(i => i.Id).ToList();
            var modifiedData = products.Select(x => new
            {
                id = x.Id,
                text = x.NameAR +"--"+x.CurrentPrice
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
