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
    public class TrainingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Trainings
        public ActionResult Index()
        {
            var trainings = db.Trainings.Include(t => t.Course).Include(t => t.Teacher).Include(t => t.Registrations);
            return View(trainings.ToList());
        }

        // GET: Trainings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Include(t => t.Registrations.Select(r => r.User))
                .FirstOrDefault(t => t.Id == id);
            if (training == null)
            {
                return HttpNotFound();
            }
            return View(training);
        }

        // GET: Trainings/Create
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Nimetus");
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Nimi");
            return View();
        }

        // POST: Trainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create([Bind(Include = "Id,CourseId,TeacherId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Training training)
        {
            if (ModelState.IsValid)
            {
                db.Trainings.Add(training);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Nimetus", training.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Nimi", training.TeacherId);
            return View(training);
        }

        // GET: Trainings/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings.Find(id);
            if (training == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Nimetus", training.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Nimi", training.TeacherId);
            return View(training);
        }

        // POST: Trainings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit([Bind(Include = "Id,CourseId,TeacherId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Training training)
        {
            if (ModelState.IsValid)
            {
                db.Entry(training).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Nimetus", training.CourseId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "Nimi", training.TeacherId);
            return View(training);
        }

        // GET: Trainings/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Training training = db.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .FirstOrDefault(t => t.Id == id);
            if (training == null)
            {
                return HttpNotFound();
            }
            return View(training);
        }

        // POST: Trainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Training training = db.Trainings.Find(id);
            db.Trainings.Remove(training);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Trainings/Register/5
        [Authorize]
        public ActionResult Register(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var training = db.Trainings.Include(t => t.Registrations).FirstOrDefault(t => t.Id == id);
            if (training == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            bool alreadyRegistered = db.Registrations
                .Any(r => r.TrainingId == id && r.ApplicationUserId == userId);
            if (alreadyRegistered)
            {
                return Content("Вы уже записаны.");
            }
            if (training.Registrations.Count >= training.MaxOsalejaid)
            {
                TempData["Error"] = "Группа заполнена";
                return RedirectToAction("Index");
            }
            var registration = new Registration
            {
                TrainingId = training.Id,
                ApplicationUserId = userId,
                Staatus = "Активна"
            };
            db.Registrations.Add(registration);
            db.SaveChanges();
            TempData["Success"] = "Вы успешно записались";
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
