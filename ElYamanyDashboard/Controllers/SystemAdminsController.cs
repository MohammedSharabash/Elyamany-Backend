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
using Microsoft.AspNet.Identity.Owin;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class SystemAdminsController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: SystemAdmins
        public async Task<ActionResult> Index()
        {
            var systemAdmins = db.SystemAdmins.Include(s => s.Role);
            return View(await systemAdmins.ToListAsync());
        }

        // GET: SystemAdmins/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemAdmin systemAdmin = await db.SystemAdmins.FindAsync(id);
            if (systemAdmin == null)
            {
                return HttpNotFound();
            }
            return View(systemAdmin);
        }

        // GET: SystemAdmins/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");
            return View();
        }

        // POST: SystemAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SystemAdmin systemAdmin)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser UserDate = null;

                var IsEmailRegistered = db.Database.SqlQuery<string>("select Email from AspNetUsers where Email like '"+systemAdmin.Email+"'").FirstOrDefault();
                if (IsEmailRegistered!=null)
                {
                    ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);

                    ViewBag.Message = "هذا البريد الالكتروني مسجل لحساب اخر";
                    return View(systemAdmin);
                }
                var user = new ApplicationUser
                {
                    UserName = systemAdmin.Email,
                    Email = systemAdmin.Email,
                    PhoneNumber = systemAdmin.PhoneNumber,
                    
                };
                var result = await UserManager.CreateAsync(user, systemAdmin.Password);
                if (result.Succeeded)
                {
                    UserDate = await UserManager.FindByEmailAsync(systemAdmin.Email);
                    if (systemAdmin.RoleId == 1)
                    {
                       await UserManager.AddToRoleAsync(UserDate.Id, "Admin");
                    }
                }
                else
                {
                    ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);

                    ViewBag.Message = "حاول مرة اخرى ببريد الكتروني جديد";
                    return View(systemAdmin);
                }
                systemAdmin.IdentityId = UserDate.Id;
                db.SystemAdmins.Add(systemAdmin);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);
            return View(systemAdmin);
        }

        // GET: SystemAdmins/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemAdmin systemAdmin = await db.SystemAdmins.FindAsync(id);
            if (systemAdmin == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);
            return View(systemAdmin);
        }

        // POST: SystemAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( SystemAdmin systemAdmin)
        {
            ApplicationUser UserDate = null;

            if (ModelState.IsValid)
            {
                SystemAdmin OldSystemAdmin = await db.SystemAdmins.FindAsync(systemAdmin.Id);
                if (OldSystemAdmin.Email!=systemAdmin.Email)
                {
                    var IsEmailRegistered = db.Database.SqlQuery<string>("select Email from AspNetUsers where Email like '" + systemAdmin.Email + "'").FirstOrDefault();
                    if (IsEmailRegistered != null)
                    {
                        ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);
                        ViewBag.Message = "هذا البريد الالكتروني مسجل لحساب اخر";
                        return View(systemAdmin);
                    }
                    UserDate = await UserManager.FindByEmailAsync(systemAdmin.Email);
                    UserDate.Email = systemAdmin.Email;
                    UserDate.UserName = systemAdmin.Email;
                    var result = await UserManager.UpdateAsync(UserDate);
                    if (result.Succeeded)
                    {
                        if (systemAdmin.RoleId == 1)
                        {
                            await UserManager.AddToRoleAsync(UserDate.Id, "Admin");
                        }
                    }
                }
                db.Entry(systemAdmin).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (!string.IsNullOrWhiteSpace(systemAdmin.Password))
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(systemAdmin.IdentityId);
                    var savingPassword = await UserManager.ResetPasswordAsync(systemAdmin.IdentityId, code, systemAdmin.Password);
                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", systemAdmin.RoleId);
            return View(systemAdmin);
        }

        // GET: SystemAdmins/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemAdmin systemAdmin = await db.SystemAdmins.FindAsync(id);
            if (systemAdmin == null)
            {
                return HttpNotFound();
            }
            return View(systemAdmin);
        }

        // POST: SystemAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            SystemAdmin systemAdmin = await db.SystemAdmins.FindAsync(id);
            var deleteUser = db.Database.ExecuteSqlCommand("delete from AspNetUsers where Id='" + systemAdmin.IdentityId + "'");
            if (deleteUser>0)
            {
                db.SystemAdmins.Remove(systemAdmin);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
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
