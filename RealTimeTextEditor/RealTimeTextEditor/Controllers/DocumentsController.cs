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
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mime;

namespace RealTimeTextEditor.Controllers
{
    public class DocumentsController : Controller
    {
        private RTTEContext db = new RTTEContext();

        [Authorize]
        // GET: Documents
        public ActionResult Index()
        {
            var user = User.Identity.GetUserName();
            // get list of all docs to which user has access, identified by their associated permissions
            var accessibleDocs = db.DocPermissions.Where(s => s.Email.Equals(user));
            var allDocuments = from m in db.Documents select m;
            List<Document> documents = new List<Document>();
            foreach (var item in accessibleDocs)
            {
                documents.Add(allDocuments.First(s => s.ID.Equals(item.DocumentID)));
            }
            ViewBag.user = user;
            return View(documents);
        }


        // GET: Documents/Details/5
        [Authorize]
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
                authorPermission.Author = true;
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
        [Authorize]
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
        [Authorize]
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


        // GET: Documents/TextEditor/5
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

            // only public documents are accessible without logging in
            if (!User.Identity.IsAuthenticated)
            {
                if (document.Public == false)
                {
                    return new HttpUnauthorizedResult();
                }
                else
                {
                    ViewBag.ReadOnly = true;
                    ViewBag.authorname = "Guest";
                    return View(document);
                }
            }

            var userID = User.Identity.GetUserId();
            //check access privileges
            var authorisedUsers = db.DocPermissions.Where(s => s.DocumentID.Equals(document.ID));
            var userName = User.Identity.GetUserName();
            authorisedUsers = authorisedUsers.Where(v => v.Email.Equals(userName));
            DocPermission authorised = null;
            if (authorisedUsers.Count() > 0)
            {
                authorised = authorisedUsers.First(v => v.Email.Equals(userName));
            }
            if (authorised == null)
            {
                if (document.Public == false)
                {
                    return new HttpUnauthorizedResult();
                }
                else
                {
                    ViewBag.ReadOnly = true;
                    ViewBag.authorname = userName;
                    return View(document);
                }
            }

            if (authorised.Edit == false)
            {
                ViewBag.ReadOnly = true;
            }
            else
            {
                ViewBag.ReadOnly = false;
            }
            // set up user profile details
            ViewBag.id = userID;
            if (db.UserProfiles.Any(s => s.UserID.Equals(userID)))
            {
                UserProfile profile = db.UserProfiles.First(s => s.UserID.Equals(userID));
                ViewBag.authorname = profile.Name;
                ViewBag.authorcolour = profile.Colour;
            }
            else
            {
                // if no profile is found, just use default settings
                ViewBag.authorname = userName;
                ViewBag.authorcolour = "FE2E2E";
            }
            var baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            ViewBag.baseUrl = baseUrl;
            return View(document);
        }


        // GET: Documents/CreateDocPermission/5
        [Authorize]
        public ActionResult CreateDocPermission(int? id)
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
            var user = User.Identity.GetUserId();
            if (user != document.UserID)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            ViewBag.docID = document.ID;
            ViewBag.docTitle = document.Title;
            return View();
        }

        // POST: Documents/CreateDocPermission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDocPermission([Bind(Include = "ID,DocumentID,Email,Read,Edit")] DocPermission docPermission)
        {
            if (ModelState.IsValid)
            {
                db.DocPermissions.Add(docPermission);
                db.SaveChanges();

                // mailer for invitation
                var document = db.Documents.First(s => s.ID.Equals(docPermission.DocumentID));
                var author = document.AuthorName;
                var title = document.Title;
                var edit = "";
                if (docPermission.Edit == true) {
                    edit = "edit";
                }
                else
                {
                    edit = "view";
                }
                var baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                var fullUrl = baseUrl + "Documents/TextEditor/" + docPermission.DocumentID;
                var body = "<p>You have been invited by " + author + " to " + edit + " the document \"" + title + "\"" + " at </p>" + fullUrl;
                var message = new MailMessage();
                message.To.Add(new MailAddress(docPermission.Email));
                message.From = new MailAddress("realtimetexteditor@gmail.com");
                message.Subject = title + " - Invitation to " + edit;
                message.Body = string.Format(body);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "realtimetexteditor@gmail.com",
                        // gmail security restrictions requires generated google app password for gmail account
                        Password = "ekofwdlgjxezakex"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);

                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateInput(false)]
        public void RtfDownload(string filename, string html)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "docs/" + filename + ".rtf";
            // convertor class uses a web browser and a windows forms richtextbox to crudely convert editor
            // contents to rtf format more suitable for downloading. This is awkward and needs to be done 
            // in a single threaded apartment, and the results are mediocre, but it works (deployment environment permitting)
            HtmlRtfConvertor convertor = new HtmlRtfConvertor();
            convertor.ThreadConvertor(path, html);
            byte[] Content = System.IO.File.ReadAllBytes(path);
            //return File(Content, MediaTypeNames.Application.Octet, path);
            //System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            Response.ClearContent();
            //response.Clear();
            Response.ContentType = "text/rtf";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".rtf");
            Response.BufferOutput = true; ;
            Response.OutputStream.Write(Content, 0, Content.Length);
            Response.End();
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
