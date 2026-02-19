using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KeelteKool.Models;
using Microsoft.AspNet.Identity;

namespace KeelteKool.Controllers
{
    public class RegistrationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Registrations/MyRegistrations
        [Authorize(Roles = "Student")]
        public ActionResult MyRegistrations()
        {
            string userId = User.Identity.GetUserId();
            var registrations = db.Registrations
                .Where(r => r.ApplicationUserId == userId)
                .Include(r => r.Training.Course)
                .Include(r => r.Training.Teacher)
                .ToList();
            return View(registrations);
        }

        // GET: Registrations (Admin only)
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var registrations = db.Registrations
                .Include(r => r.Training.Course)
                .Include(r => r.User)
                .ToList();
            return View(registrations);
        }

        // POST: Registrations/UpdateStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatus(int id, string status)
        {
            var registration = db.Registrations.Find(id);
            if (registration != null)
            {
                registration.Staatus = status;
                db.SaveChanges();
            }
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
