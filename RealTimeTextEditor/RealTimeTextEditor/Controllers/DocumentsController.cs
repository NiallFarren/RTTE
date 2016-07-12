using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RealTimeTextEditor.Models;
using Microsoft.AspNet.Identity;
using RealTimeTextEditor.Context;
using System.IO;

namespace RealTimeTextEditor.Controllers
{
    public class DocumentsController : Controller
    {
        private RTTEContext db = new RTTEContext();

        [Authorize]
        // GET: Documents
        public ActionResult Index()
        {

            var user = User.Identity.GetUserId();
            var documents = from m in db.Documents select m;
            documents = documents.Where(s => s.UserID.Equals(user));
            return View(documents.ToList());
        }


        // GET: Documents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,AuthorName,Title,Public,_Date")] Document document)
        {
            if (ModelState.IsValid)
            {
                document.UserID = User.Identity.GetUserId();
                document.AuthorName = User.Identity.GetUserName();
                db.Documents.Add(document);
                DocPermission authorPermission = new DocPermission();
                authorPermission.DocumentID = document.ID;
                authorPermission.Email = User.Identity.GetUserName();
                authorPermission.Read = true;
                authorPermission.Edit = true;
                db.DocPermissions.Add(authorPermission); 
                db.SaveChanges();
                // create blank file
                String filename = document.UserID + document.Title;
                string path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".txt";
                CreateFile cf = new CreateFile();
                bool success = cf.Create(path);

                return RedirectToAction("Index");
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,AuthorName,Title,Public,_Date")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult TextEditor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            var userID = User.Identity.GetUserId();
            var profile = db.UserProfiles.First(s => s.UserID.Equals(userID));
            ViewBag.id = userID;
            ViewBag.authorname = profile.Name;
            ViewBag.authorcolour = profile.Colour;
            return View(document);
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
