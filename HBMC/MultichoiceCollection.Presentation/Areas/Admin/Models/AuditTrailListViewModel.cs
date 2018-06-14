using MultichoiceCollection.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultichoiceCollection.Presentation.Areas.Admin.Models
{
    public class AuditTrailListViewModel
    {
        public List<AuditTrail> AuditTrails { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}