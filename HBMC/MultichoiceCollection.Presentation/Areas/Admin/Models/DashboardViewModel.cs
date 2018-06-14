using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultichoiceCollection.Presentation.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            ChartInfo = new List<ChartNode>();
        }

        public int TransactionCount { get; set; }
        public int AgentsCount { get; set; }
        public decimal AmountProcessed { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<ChartNode> ChartInfo { get; set; }
    }

    public class ChartNode
    {
        public string country { get; set; }
        public decimal visits { get; set; }
        public string color { get; set; }
    }
}