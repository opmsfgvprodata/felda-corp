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

namespace MVC_SYSTEM.Controllers
{
    public class CostCentreController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();

        // GET: CostCentre
        public async Task<ActionResult> Index()
        {
            return View(await db.tbl_CostCentre.ToListAsync());
        }

        // GET: CostCentre/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CostCentre tbl_CostCentre = await db.tbl_CostCentre.FindAsync(id);
            if (tbl_CostCentre == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CostCentre);
        }

        // GET: CostCentre/Create
        public ActionResult Create()
        {
            var fld_LadangID = new SelectList(db.tbl_Ladang.Where(x => x.fld_SyarikatID == 2 && x.fld_Deleted == false), "fld_ID", "fld_LdgName").ToList();
            var fld_KodKtgri = new SelectList(db.tbl_KategoriAktiviti.Where(x => x.fld_SyarikatID == 2 && x.fld_Deleted == false), "fld_KodKategori", "fld_Kategori").ToList();
            ViewBag.fld_KodKtgri = fld_KodKtgri;
            ViewBag.fld_LadangID = fld_LadangID;
            return View();
        }

        // POST: CostCentre/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "fld_ID,fld_CostCentre,fld_KodKtgri,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID,fld_Deleted")] tbl_CostCentre tbl_CostCentre)
        {
            if (ModelState.IsValid)
            {
                db.tbl_CostCentre.Add(tbl_CostCentre);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tbl_CostCentre);
        }

        // GET: CostCentre/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CostCentre tbl_CostCentre = await db.tbl_CostCentre.FindAsync(id);
            if (tbl_CostCentre == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CostCentre);
        }

        // POST: CostCentre/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "fld_ID,fld_CostCentre,fld_KodKtgri,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID,fld_Deleted")] tbl_CostCentre tbl_CostCentre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_CostCentre).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tbl_CostCentre);
        }

        // GET: CostCentre/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CostCentre tbl_CostCentre = await db.tbl_CostCentre.FindAsync(id);
            if (tbl_CostCentre == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CostCentre);
        }

        // POST: CostCentre/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tbl_CostCentre tbl_CostCentre = await db.tbl_CostCentre.FindAsync(id);
            db.tbl_CostCentre.Remove(tbl_CostCentre);
            await db.SaveChangesAsync();
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
