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

namespace RealTimeTextEditor.Controllers
{
    [Authorize]
    public class DocPermissionsController : Controller
    {
        private RTTEContext db = new RTTEContext();

        // GET: DocPermissions
        public ActionResult Index()
        {
            var docPermissions = db.DocPermissions.Include(d => d.Document);
            return View(docPermissions.ToList());
        }

        // GET: DocPermissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocPermission docPermission = db.DocPermissions.Find(id);
            if (docPermission == null)
            {
                return HttpNotFound();
            }
            return View(docPermission);
        }

        // GET: DocPermissions/Create
        public ActionResult Create()
        {
            ViewBag.DocumentID = new SelectList(db.Documents, "ID", "UserID");
            return View();
        }

        // POST: DocPermissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DocumentID,Email,Read,Edit")] DocPermission docPermission)
        {
            if (ModelState.IsValid)
            {
                db.DocPermissions.Add(docPermission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DocumentID = new SelectList(db.Documents, "ID", "UserID", docPermission.DocumentID);
            return View(docPermission);
        }

        // GET: DocPermissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocPermission docPermission = db.DocPermissions.Find(id);
            if (docPermission == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocumentID = new SelectList(db.Documents, "ID", "UserID", docPermission.DocumentID);
            return View(docPermission);
        }

        // POST: DocPermissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DocumentID,Email,Read,Edit")] DocPermission docPermission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docPermission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DocumentID = new SelectList(db.Documents, "ID", "UserID", docPermission.DocumentID);
            return View(docPermission);
        }

        // GET: DocPermissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocPermission docPermission = db.DocPermissions.Find(id);
            if (docPermission == null)
            {
                return HttpNotFound();
            }
            return View(docPermission);
        }

        // POST: DocPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocPermission docPermission = db.DocPermissions.Find(id);
            db.DocPermissions.Remove(docPermission);
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
