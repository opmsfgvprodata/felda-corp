using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ConfigModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.Attributes;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin")]
    public class SystemConfigsController : Controller
    {
        GetConfig getconfig = new GetConfig();
        GetIdentity getidentity = new GetIdentity();
        errorlog geterror = new errorlog();
        private MVC_SYSTEM_Config db = new MVC_SYSTEM_Config();

        // GET: SystemConfigs
        public ActionResult Index(string filter = "", int fldConfigID = 0, int page = 1, string sort = "fldConfigDesc", string sortdir = "DESC")
        {
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
            ViewBag.SystemConfig = "class = active";
            int pageSize = int.Parse(getconfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tblSystemConfig>();
            ViewBag.fldConfigID = new SelectList(db.tblSystemConfigs.Where(x => x.fldDeleted == false), "fldConfigID", "fldConfigDesc");
            ViewBag.filter = filter;
            if (filter == "")
            {
                records.Content = dbview.tblSystemConfigs
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tblSystemConfigs.Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            else
            {
                records.Content = dbview.tblSystemConfigs.Where(x => x.fldConfigDesc.Contains(filter) || x.fldConfigValue.Contains(filter))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = dbview.tblSystemConfigs.Where(x => x.fldConfigDesc.Contains(filter)).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }
            return View(records);
        }

        // GET: SystemConfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ConfigModels.tblSystemConfig tblSystemConfig = db.tblSystemConfigs.Find(id);
            if (tblSystemConfig == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Details", tblSystemConfig);
        }

        // GET: SystemConfigs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fldConfigID,fldConfigValue,fldConfigDesc,fldFlag1,fldFlag2,fldDeleted")] ConfigModels.tblSystemConfig tblSystemConfig)
        {
            if (ModelState.IsValid)
            {
                db.tblSystemConfigs.Add(tblSystemConfig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblSystemConfig);
        }

        // GET: SystemConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ConfigModels.tblSystemConfig tblSystemConfig = db.tblSystemConfigs.Find(id);
            if (tblSystemConfig == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("Edit", tblSystemConfig);
        }

        // POST: SystemConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind(Include = "fldConfigID,fldConfigValue,fldConfigDesc,fldFlag1,fldFlag2,fldDeleted")] ConfigModels.tblSystemConfig tblSystemConfig)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = db.tblSystemConfigs.Find(id);
                    getdata.fldConfigDesc = tblSystemConfig.fldConfigDesc;
                    getdata.fldConfigValue = tblSystemConfig.fldConfigValue;
                    db.Entry(getdata).State = EntityState.Modified;
                    db.SaveChanges();
                    var getid = id;
                    return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = tblSystemConfig.fldConfigDesc, data2 = tblSystemConfig.fldConfigValue });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
            }
            //return View(tblSystemConfig);
        }

        // GET: SystemConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ConfigModels.tblSystemConfig tblSystemConfig = db.tblSystemConfigs.Find(id);
            if (tblSystemConfig == null)
            {
                return RedirectToAction("Index");
            }
            return View(tblSystemConfig);
        }

        // POST: SystemConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfigModels.tblSystemConfig tblSystemConfig = db.tblSystemConfigs.Find(id);
            db.tblSystemConfigs.Remove(tblSystemConfig);
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
