using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MultichoiceCollection.Common.Entities.Enum;
using MultichoiceCollection.Models.Repositories.Context;
using MultichoiceCollection.Presentation.Attributes;
using MultichoiceCollection.Presentation.Models;
using MultichoiceCollection.Presentation.Services;
using MultichoiceCollection.Services.Implementations;

namespace MultichoiceCollection.Presentation.Controllers
{
    [CustomAuthorize]
    public class HomeController : BaseController
    {
        readonly ApiRequestService _request;
        private readonly AppDbContext _appDbContext;
        public HomeController()
        {
             _request = new ApiRequestService();
            _appDbContext = new AppDbContext();

        }
        public ActionResult Index()
        {
            var user = _appDbContext.Users.First(u => u.UserName == User.Identity.Name);
            if (user.ShouldChangePassword)
            {
                ShowMessage("Please update your password to continu", AlertType.Danger);
                return RedirectToAction("ChangePassword", "Account");
            }
            var accountNumber = user.AccountNumber;
            // ViewBag.TotalSales = _appDbContext.Transactions.Sum(tr => tr.amount).ToString("N");
            if (_appDbContext.Transactions.Any(t => t.accountNumber == accountNumber))
            {

                ViewBag.TotalSales = _appDbContext.Transactions.Where(t=>t.accountNumber == accountNumber).Sum(tr => tr.amount);
                ViewBag.PrintedReceipts = _appDbContext.Transactions.Where(t => t.accountNumber == accountNumber).Count(tr => tr.PrintedReceipt);
                ViewBag.LatestTransactions = _appDbContext.Transactions.Where(t => t.accountNumber == accountNumber).OrderByDescending(tr => tr.id).Take(10)
                    .ToList();
            }

            ViewBag.CurrentDashboard = "current";
            return View();
        }
        public ActionResult BouquetTypes()
        {
            ViewBag.Types = "current";
            var response = new List<BouquetTypeModel>();
            try
            {
                
                 response = _request.MakeRequest<List<BouquetTypeModel>>(ApiConstantService.TYPES);
                return View(response);
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }

        }
        public ActionResult Bouquets(string type)
        {
            ViewBag.Bouquets = "current";
            if (string.IsNullOrEmpty(type))
            {
                ShowMessage("Type of bouquet must be selected", AlertType.Danger);
                return RedirectToAction("Index");
            }
            if (!type.Equals("gotv") && !type.Equals("dstv"))
            {
                ShowMessage("Bouquet type can only be gotv or dstv", AlertType.Danger);
                return RedirectToAction("Index");
            }
                var response = new List<BouquetModel>();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/bouquet";
                 response = _request.MakeRequest<List<BouquetModel>>(url);
                return View(response);
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }

        }

        public ActionResult Accounts(string type, string cardNumber)
        {
            ViewBag.Accounts = "current";
            if (string.IsNullOrEmpty(type))
            {
                ShowMessage("Type of bouquet must be selected", AlertType.Danger);
                return RedirectToAction("Index");
            }
            if (!type.Equals("gotv") && !type.Equals("dstv"))
            {
                ShowMessage("Bouquet type can only be gotv or dstv", AlertType.Danger);
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(cardNumber))
            {
                ShowMessage("Smart card number is required.", AlertType.Danger);
                return RedirectToAction("Index");
            }
            var response = new CustomerAccountModel();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/accounts/" + cardNumber;
                response = _request.MakeRequest<CustomerAccountModel>(url);
                return View(response);
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }
        }

        public ActionResult BouquetDetails(string type, string cardNumber)
        {
            ViewBag.BouquetDetails = "current";
            if (string.IsNullOrEmpty(type))
            {
                ShowMessage("Type of bouquet must be selected", AlertType.Danger);
                return RedirectToAction("Index");
            }
            if (!type.Equals("gotv") && !type.Equals("dstv"))
            {
                ShowMessage("Bouquet type can only be gotv or dstv", AlertType.Danger);
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(cardNumber))
            {
                ShowMessage("Smart card number is required.", AlertType.Danger);
                return RedirectToAction("Index");
            }
            var response = new CustomerDetailsModel();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/details/" + cardNumber;
                response = _request.MakeRequest<CustomerDetailsModel>(url);
                return View(response);
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }
        }
    }
}