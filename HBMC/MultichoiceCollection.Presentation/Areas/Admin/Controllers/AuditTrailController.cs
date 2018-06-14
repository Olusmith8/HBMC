using MultichoiceCollection.Models.Repositories.Context;
using MultichoiceCollection.Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultichoiceCollection.Presentation.Areas.Admin.Controllers
{
    public class AuditTrailController : Controller
    {
        // GET: Admin/AuditTrail
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 50)
        {
            using (var context = new AppDbContext())
            {
                ViewBag.AuditTrail = "active";

                var offset = (page - 1) * pageSize;
                var query = startDate.HasValue && endDate.HasValue ? context.AuditTrails.Where(a => a.Date <= startDate && a.Date >= endDate) : context.AuditTrails.Where(a => a.Id > 0);
                var auditTrail = query.OrderByDescending(a=>a.Id).Skip(offset).Take(pageSize).ToList();
                var total = query.Count();
                return View(new AuditTrailListViewModel {AuditTrails = auditTrail, Page = page, PageSize = pageSize , TotalCount = total});
            }

        }
    }
}