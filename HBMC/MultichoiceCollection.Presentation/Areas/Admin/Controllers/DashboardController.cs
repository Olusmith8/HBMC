using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MultichoiceCollection.Common.Entities;
using MultichoiceCollection.Common.Entities.Base;
using MultichoiceCollection.Common.Entities.Enum;
using MultichoiceCollection.Models.Repositories.Context;
using MultichoiceCollection.Presentation.Areas.Admin.Models;
using MultichoiceCollection.Presentation.Attributes;
using MultichoiceCollection.Presentation.Controllers;
using MultichoiceCollection.Presentation.Models;
using MultichoiceCollection.Presentation.Services;
using MultichoiceCollection.Services.Implementations;

namespace MultichoiceCollection.Presentation.Areas.Admin.Controllers
{
    [AdminAuthorize(Roles = RoleNames.RoleAdmin)]
    public class DashboardController : AdminBaseController
    {
        readonly ApiRequestService request;
        public DashboardController()
        {
             request = new ApiRequestService();
        }

        public async Task<ActionResult> Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate == null)
            {
                startDate = DateTime.Now.AddDays(-7);
            }
            if (endDate == null) endDate = DateTime.Now;

            var vm = new DashboardViewModel();
            using (var context = new AppDbContext())
            {
                if (context.Transactions.Any())
                {
                    vm.TransactionCount = context.Transactions.Count(t => t.success);
                    vm.AmountProcessed = context.Transactions.Where(t => t.success).Sum(t => t.amount);
                }

                // var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AppDbContext()));
                vm.AgentsCount = (await roleManager.FindByNameAsync(RoleNames.RoleCustomer)).Users.Count;

                //amChart
                var chatTx = context.Transactions.Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate);

                var chartVm = new Dictionary<string, ChartNode>();
                var count = (startDate.Value - endDate.Value).Days;
                var date = startDate.Value;

                for (var i = 0; i < count; i++)
                {
                    var c = i % 10;
                    chartVm[date.ToShortDateString()] = new ChartNode { country = date.AddDays(i).ToShortDateString(), color = $"#FF{c}60{c}"};

                }
                foreach (var transaction in chatTx)
                {
                    if (!chartVm.ContainsKey(transaction.CreatedDate.ToShortDateString()))
                    {
                        chartVm[transaction.CreatedDate.ToShortDateString()] = new ChartNode { country = transaction.CreatedDate.ToShortDateString(), color = $"#FF9604" };
                    }
                    var node = chartVm[transaction.CreatedDate.ToShortDateString()];
                    node.visits += transaction.amount;

                    chartVm[transaction.CreatedDate.ToShortDateString()] = node;
                }

                vm.ChartInfo = chartVm.Select(c => c.Value).ToList();

            }
            ViewBag.CurrentDashboard = "current";
            return View(vm);
        }

        public ActionResult Transactions(int page =1, DateTime? startDate = null, DateTime? endDate = null, string smartCardNumber = null)
        {
            if (page < 1) page = 1;

            ViewBag.Transactions = "current";
            using (var context = new AppDbContext())
            {
                var query = context.Transactions.Where(t=>t.success);
                if (endDate.HasValue)
                {
                    query = query.Where(q => q.CreatedDate >= startDate && q.CreatedDate <= endDate);
                }
                if (smartCardNumber != null)
                {
                    query = query.Where(t => t.customerNumber == smartCardNumber);
                }

                var total = query.Count();

                var offset = (page - 1) * 50;
                var transactions = query.OrderByDescending(t => t.CreatedDate).Skip(offset).Take(50);
                var vm = new TransactionViewModel
                {
                    Transactions = transactions.ToList(),
                    PageSize = 50,
                    Page = page,
                    Total = total,
                    SmartCardNumber = smartCardNumber
                };

                return View(vm);
            }

        }

        public ActionResult Reports(int page = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (page < 1) page = 1;

            ViewBag.Reports = "current";
            using (var context = new AppDbContext())
            {
                var query = context.Transactions.Where(t => t.success);
                if (endDate.HasValue)
                {
                    query = query.Where(q => q.CreatedDate <= startDate && q.CreatedDate <= endDate);
                }

                var offset = (page - 1) * 50;
                var transactions = query.OrderByDescending(t => t.CreatedDate).Skip(offset).Take(50);//todo do pagination
                var total = query.Count();
                return View(new ReportViewModel
                {
                    Transactions = transactions.ToList(),
                    StartDate = startDate,
                    EndDate = endDate,
                    Total = total,
                    Page = page,
                    PageSize = 50
                });
            }
        }
        

        public void Download(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var context = new AppDbContext())
            {
                var query = context.Transactions.Where(t => t.success);
                if (endDate.HasValue)
                {
                    query = query.Where(q => q.CreatedDate <= startDate && q.CreatedDate <= endDate);
                }
                
                var sb = new StringBuilder();
                var list = query.ToList();
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", "S/No", "Agent Till", "Amount", "Fees ", "Customer Number",
                    "Phone", "Trans Date", "Audit Ref No", "Device No", "Business Type", "Creation Date", Environment.NewLine);
                var sn = 0;
                foreach (var item in list)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", ++sn, item.accountNumber,
                        item.amount,
                        item.transFees, item.customerNumber, item.mobileNumber, item.transactionDate,
                        item.auditReferenceNo, item.deviceNumber, item.businessType,
                        item.CreatedDate.ToShortDateString(),
                        Environment.NewLine);
                }
                //Get Current Response  
                var response = System.Web.HttpContext.Current.Response;
                response.BufferOutput = true;
                response.Clear();
                response.ClearHeaders();
                response.ContentEncoding = Encoding.Unicode;
                response.AddHeader("content-disposition", "attachment;filename=report.CSV ");
                response.ContentType = "text/plain";
                response.Write(sb.ToString());
                response.End();
            }
           
        }
    }
}