using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.ControllersAPI
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using MVC_SYSTEM.AuthModels;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<tblUser>("tblUsers");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class tblUsersController : ODataController
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

        // GET: odata/tblUsers
        [EnableQuery]
        public IQueryable<tblUser> GettblUsers()
        {
            return db.tblUsers;
        }

        // GET: odata/tblUsers(5)
        [EnableQuery]
        public SingleResult<tblUser> GettblUser([FromODataUri] int key)
        {
            return SingleResult.Create(db.tblUsers.Where(tblUser => tblUser.fldUserID == key));
        }

        // PUT: odata/tblUsers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<tblUser> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tblUser tblUser = await db.tblUsers.FindAsync(key);
            if (tblUser == null)
            {
                return NotFound();
            }

            patch.Put(tblUser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblUserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tblUser);
        }

        // POST: odata/tblUsers
        public async Task<IHttpActionResult> Post(tblUser tblUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblUsers.Add(tblUser);
            await db.SaveChangesAsync();

            return Created(tblUser);
        }

        // PATCH: odata/tblUsers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<tblUser> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tblUser tblUser = await db.tblUsers.FindAsync(key);
            if (tblUser == null)
            {
                return NotFound();
            }

            patch.Patch(tblUser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblUserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tblUser);
        }

        // DELETE: odata/tblUsers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            tblUser tblUser = await db.tblUsers.FindAsync(key);
            if (tblUser == null)
            {
                return NotFound();
            }

            db.tblUsers.Remove(tblUser);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblUserExists(int key)
        {
            return db.tblUsers.Count(e => e.fldUserID == key) > 0;
        }
    }
}
