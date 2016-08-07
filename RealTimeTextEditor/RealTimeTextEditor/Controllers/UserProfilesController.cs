using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RealTimeTextEditor.Context;
using RealTimeTextEditor.Models;
using Microsoft.AspNet.Identity;

namespace RealTimeTextEditor.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private RTTEContext db = new RTTEContext();

        // GET: UserProfiles
        public ActionResult Index()
        {
            return RedirectToAction("Details");
           // return View(db.UserProfiles.ToList());
        }

        // GET: UserProfiles/Details
        public ActionResult Details()
        {
            var Userid = User.Identity.GetUserId();
            if (db.UserProfiles.Any(s => s.UserID.Equals(Userid)))
            {
                UserProfile profile = db.UserProfiles.First(s => s.UserID.Equals(Userid));
                return View(profile);
            }
            else
            {
                //if no profile exists, direct to create one
                return RedirectToAction("Create");
            }

        }

        // GET: UserProfiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,Name,Colour")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                userProfile.UserID = User.Identity.GetUserId();
                db.UserProfiles.Add(userProfile);
                db.SaveChanges();
                return RedirectToAction("Details/" + userProfile.ID);
            }

            return View(userProfile);
        }

        // GET: UserProfiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,Name,Colour")] UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                userProfile.UserID = User.Identity.GetUserId();
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details/" + userProfile.ID);
            }
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userProfile = db.UserProfiles.Find(id);
            db.UserProfiles.Remove(userProfile);
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
