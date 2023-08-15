using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Attributes;

namespace MVC_SYSTEM.Controllers
{
    //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Resource,Viewer")]
    public class MessagingController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetIdentity GetIdentity = new GetIdentity();
        private ChangeTimeZone GetTime = new ChangeTimeZone();
        // GET: Messaging
        public async Task<ActionResult> Index(string FindText, int? Status)
        {
            ViewBag.Messaging = "class = active";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? GetUserID = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, GetUserID, User.Identity.Name);
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            ViewBag.Status = StatusList;
            if (string.IsNullOrEmpty(FindText))
            {
                if (Status == null)
                {
                    return View(await db.tbl_Messaging.ToListAsync());
                }
                else
                {
                    bool ActStatus = true;
                    switch (Status)
                    {
                        case 1:
                            ActStatus = true;
                            break;
                        case 2:
                            ActStatus = false;
                            break;
                    }

                    return View(await db.tbl_Messaging.Where(x => x.fld_Active == ActStatus).ToListAsync());
                }
            }
            else
            {
                return View(await db.tbl_Messaging.Where(x=> x.fld_Msg.Contains(FindText)).ToListAsync());
            }
        }

        // GET: Messaging/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Messaging tbl_Messaging = await db.tbl_Messaging.FindAsync(id);
            if (tbl_Messaging == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Messaging);
        }

        // GET: Messaging/Create
        public ActionResult Create()
        {
            ViewBag.Messaging = "class = active";
            return View();
        }

        // POST: Messaging/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "fld_Msg,fld_Title")] tbl_Messaging tbl_Messaging)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? GetUserID = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, GetUserID, User.Identity.Name);

            tbl_Messaging.fld_Active = true;
            tbl_Messaging.fld_CreatedBy = GetUserID;
            tbl_Messaging.fld_CreatedDT = GetTime.gettimezone();
            tbl_Messaging.fld_NegaraID = NegaraID;
            tbl_Messaging.fld_SyarikatID = SyarikatID;
            tbl_Messaging.fld_WilayahID = 0;
            tbl_Messaging.fld_LadangID = 0;
            tbl_Messaging.fld_ModifiedBy = null;
            tbl_Messaging.fld_ModifiedDT = null;
            tbl_Messaging.fld_Purpose = 1;
            if (ModelState.IsValid)
            {
                db.tbl_Messaging.Add(tbl_Messaging);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbl_Messaging);
        }

        // GET: Messaging/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            ViewBag.Messaging = "class = active";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? GetUserID = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, GetUserID, User.Identity.Name);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Messaging tbl_Messaging = await db.tbl_Messaging.FindAsync(id);
            if (tbl_Messaging == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> StatusList = new List<SelectListItem>();
            var GetStatus = tbl_Messaging.fld_Active;
            string ActiveStatus = "1";
            switch (GetStatus)
            {
                case true:
                    ActiveStatus = "1";
                    break;
                case false:
                    ActiveStatus = "2";
                    break;
            }
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", ActiveStatus).ToList();
            ViewBag.Status = StatusList;

            return View(tbl_Messaging);
        }

        // POST: Messaging/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "fld_ID,fld_Msg,fld_Title")] tbl_Messaging tbl_Messaging, int? Status)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? GetUserID = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, GetUserID, User.Identity.Name);

            var Get_tbl_Messaging = db.tbl_Messaging.Find(tbl_Messaging.fld_ID);
            Get_tbl_Messaging.fld_Msg = tbl_Messaging.fld_Msg;
            Get_tbl_Messaging.fld_Title = tbl_Messaging.fld_Title;
            Get_tbl_Messaging.fld_ModifiedBy = GetUserID;
            Get_tbl_Messaging.fld_ModifiedDT = GetTime.gettimezone();
            bool ActStatus = true;
            switch (Status)
            {
                case 1:
                    ActStatus = true;
                    break;
                case 2:
                    ActStatus = false;
                    break;
            }
            Get_tbl_Messaging.fld_Active = ActStatus;
            if (ModelState.IsValid)
            {
                db.Entry(Get_tbl_Messaging).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Get_tbl_Messaging);
        }

        // GET: Messaging/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Messaging tbl_Messaging = await db.tbl_Messaging.FindAsync(id);
            if (tbl_Messaging == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Messaging);
        }

        // POST: Messaging/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            tbl_Messaging tbl_Messaging = await db.tbl_Messaging.FindAsync(id);
            db.tbl_Messaging.Remove(tbl_Messaging);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult _ShowBroadcast()
        {
            return View(db.tbl_Messaging.Where(x => x.fld_Active == true).ToList());
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
