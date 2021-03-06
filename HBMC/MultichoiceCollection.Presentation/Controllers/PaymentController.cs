﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using MultichoiceCollection.Common.Entities;
using MultichoiceCollection.Common.Entities.Enum;
using MultichoiceCollection.Models.Repositories.Context;
using MultichoiceCollection.Models.Repositories.Implementations;
using MultichoiceCollection.Presentation.Models;
using MultichoiceCollection.Presentation.Services;
using MultichoiceCollection.Services.Implementations;

namespace MultichoiceCollection.Presentation.Controllers
{
    [Authorize]
    public class PaymentController : BaseController
    {
       private readonly ApiRequestService _request;
       private AppDbContext _appDbContext;
        public PaymentController()
        {
            _request = new ApiRequestService();
            this._appDbContext = new AppDbContext();
        }
        // GET: Payment
         public ActionResult Index(string id)
         {
            var username = User.Identity.Name;
             if (string.IsNullOrEmpty(id))
             {
                this.ShowMessage("Payment type is required.", AlertType.Danger);
                 return RedirectToAction("Index", "Home");
             }
             ViewBag.CurrentPayment = "current";
             if (id == "customerNumber")
             {
                ViewBag.PaymentType = "makePaymentCustomerNumberModal";
                 ViewBag.PayEl = CustomerAccountPayEl;
             }else if (id == "smartCardNumber")
             {
                ViewBag.PaymentType = "makePaymentSmartCardNumberModal";
                 ViewBag.PayEl = SmartCardPayEl;
             }
            return View();
         }
       
        public ActionResult MakePaymentCustomerNumber()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult MakePaymentCustomerNumber(
     string type, string productKey, string customernumber, string basketId,
     decimal amount, int invoicePeriod, string emailAddress,
     string mobileNumber, string paymentMode
     )
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(productKey) ||
                       string.IsNullOrEmpty(customernumber) ||
                       string.IsNullOrEmpty(emailAddress) ||
                       string.IsNullOrEmpty(mobileNumber)
                       || string.IsNullOrEmpty(paymentMode))
            {
                ShowMessage("All the fields are required.", AlertType.Warning);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.MakePaymentCustomerNumber = "current";
            var response = new MakePaymentCustomerNumberResponseModel();
            try
            {
                var user = _appDbContext.Users.First(u => u.UserName == User.Identity.Name);
                var accountNumber = user.AccountNumber;

                var url = ApiConstantService.BASE_URL + type.ToLower() + "/payment/" + productKey + "/" + customernumber;
                var headers = new NameValueCollection();
                headers.Add("Authorization", "bearer " + this.UserInfo.accessToken);
                var postData = new Dictionary<string, string>();
                postData.Add("amount", amount + "");
                postData.Add("invoicePeriod", invoicePeriod + "");
                postData.Add("emailAddress", emailAddress);
                postData.Add("accountNumber", accountNumber);
                postData.Add("mobileNumber", mobileNumber);
                postData.Add("paymentMode", paymentMode);
                response = _request.MakeRequest<MakePaymentCustomerNumberResponseModel>(url, postData, HttpVerb.POST, headers);
                if (response != null)
                {
                    var transactionEnquiriesUrl = ApiConstantService.BASE_URL + "transactions/" +
                                                  response.transactionNumber;
                    var transactionEnquiries = _request.MakeRequest<TransactionsResponseModel>(transactionEnquiriesUrl);
                    var transaction = _appDbContext.Transactions.Add(new Transaction
                    {
                        accountNumber = transactionEnquiries.accountNumber,
                        transactionDate = transactionEnquiries.transactionDate,
                        apiClientId = transactionEnquiries.apiClientId,
                        customerNumber = transactionEnquiries.customerNumber,
                        deviceNumber = transactionEnquiries.deviceNumber,
                        packageCode = transactionEnquiries.packageCode,
                        businessType = transactionEnquiries.businessType,
                        emailAddress = transactionEnquiries.emailAddress,
                        mobileNumber = transactionEnquiries.mobileNumber,
                        amount = transactionEnquiries.amount,
                        transFees = transactionEnquiries.transFees,
                        posted = transactionEnquiries.posted,
                        auditReferenceNo = transactionEnquiries.auditReferenceNo,
                        success = transactionEnquiries.success,
                        url = transactionEnquiries.url,
                        CreatedDate = DateTime.UtcNow
                    });
                    if (this._appDbContext.SaveChanges() > 0)
                    {
                        ViewBag.Id = transaction.id;
                    }
                }

                return View(response);
            }
            catch (Exception exception)
            {
                this.ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }
        }

        public ActionResult MakePaymentSmartCardNumber()
        {
            return RedirectToAction("Index", "Home");
        }

        public const string CustomerAccountPayEl = "can";
        public const string SmartCardPayEl = "sc";

        public ActionResult ValidateNumberForPayment(string type, string cardNumber, string payel)
        {
            if (string.IsNullOrEmpty(type))
            {
                this.ShowMessage("Type of bouquet must be selected", AlertType.Danger);
                return RedirectToAction("Index", new{id= type });
            }
            if (!type.Equals("gotv") && !type.Equals("dstv"))
            {
                this.ShowMessage("Bouquet type can only be gotv or dstv", AlertType.Danger);
                return RedirectToAction("Index", new { id = type });
            }
            if (string.IsNullOrEmpty(cardNumber))
            {
                this.ShowMessage("Smart card number is required.", AlertType.Danger);
                return RedirectToAction("Index", new { id = type });
            }

            var response = new CustomerAccountModel();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/accounts/" + cardNumber;
                response = _request.MakeRequest<CustomerAccountModel>(url);
                if (response == null)
                {
                    ShowMessage("Invalid card number", AlertType.Danger);
                    return RedirectToAction("Index", new { id = type });
                }
                return RedirectToAction("MakePayment", new {type, cardNumber, payel});
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                return RedirectToAction("Index", new { id = payel });
            }
        }

        public ActionResult MakePayment(string type, string payel, string cardNumber = "")
        {
            if (string.IsNullOrEmpty(payel))
            {
                return RedirectToAction("Index", new {id = type});
            }
            var response = new List<BouquetModel>();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/bouquet";
                response = _request.MakeRequest<List<BouquetModel>>(url);

            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, AlertType.Danger);
                response = new List<BouquetModel>();
                //return RedirectToAction("Index");
            }

            var vm = new MakePaymentViewModel {Type = type, Bouquets = response, PayEl = payel, CardNumber = cardNumber};
            return View($"MakePayment_{payel}", vm);
        }

        [HttpPost]
        public ActionResult MakePaymentSmartCardNumber(
            string type, string productKey, string smartcardnumber,
            decimal amount, int invoicePeriod, string emailAddress, 
            string mobileNumber, string paymentDescription, string paymentMode
            )
        {
                   if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(productKey) ||
                       string.IsNullOrEmpty(smartcardnumber) ||
                       string.IsNullOrEmpty(emailAddress) || 
                       string.IsNullOrEmpty(mobileNumber)
                       || string.IsNullOrEmpty(paymentDescription) || string.IsNullOrEmpty(paymentMode))
                   {
                ShowMessage("All the fields are required.", AlertType.Warning);
                return RedirectToAction("Index", "Home");
            }

            var user = _appDbContext.Users.First(u => u.UserName == User.Identity.Name);
            var accountNumber = user.AccountNumber;

            ViewBag.MakePaymentSmartCardNumber = "current";
            var response = new MakePaymentSmartCardNumberResponseModel();
            try
            {
                var url = ApiConstantService.BASE_URL + type + "/payment/" + productKey + "/" + smartcardnumber;
                var headers = new NameValueCollection();
                headers.Add("Authorization", "Bearer " + this.UserInfo.accessToken);
                var postData = new Dictionary<string, string>
                {
                    {"amount", amount + ""},
                    {"invoicePeriod", invoicePeriod + ""},
                    {"emailAddress", emailAddress},
                    {"accountNumber", accountNumber},
                    {"mobileNumber", mobileNumber},
                    {"paymentDescription", paymentDescription},
                    {"paymentMode", paymentMode}
                };
                response = _request.MakeRequest<MakePaymentSmartCardNumberResponseModel>(url, postData, HttpVerb.POST, headers);
                if (response != null)
                {
                    var transactionEnquiriesUrl = ApiConstantService.BASE_URL + "transactions/" +
                                                  response.transactionNumber;
                    var transactionEnquiries = _request.MakeRequest<TransactionsResponseModel>(transactionEnquiriesUrl);
                    var transaction = _appDbContext.Transactions.Add(new Transaction
                    {
                        accountNumber = transactionEnquiries.accountNumber,
                        transactionDate = transactionEnquiries.transactionDate,
                        apiClientId = transactionEnquiries.apiClientId,
                        customerNumber = transactionEnquiries.customerNumber,
                        deviceNumber = transactionEnquiries.deviceNumber,
                        packageCode = transactionEnquiries.packageCode,
                        businessType = transactionEnquiries.businessType,
                        emailAddress = transactionEnquiries.emailAddress,
                        mobileNumber = transactionEnquiries.mobileNumber,
                        amount = transactionEnquiries.amount,
                        transFees = transactionEnquiries.transFees,
                        posted = transactionEnquiries.posted,
                        auditReferenceNo = transactionEnquiries.auditReferenceNo,
                        success = transactionEnquiries.success,
                        url = transactionEnquiries.url,
                        CreatedDate = DateTime.UtcNow
                    });

                    if (this._appDbContext.SaveChanges() > 0)
                    {
                        ViewBag.Id = transaction.id;
                    }
                }

                return View(response);
            }
            catch (Exception exception)
            {
                this.ShowMessage(exception.Message, AlertType.Danger);
                return View(response);
            }
        }


        public ActionResult Receipt(int id)
        {

            var transaction = _appDbContext.Transactions.Find(id);
            var user = _appDbContext.Users.First(u => u.UserName == User.Identity.Name);

            if (transaction == null)
            {
                ShowMessage("Transaction was not found.", AlertType.Warning);
                return RedirectToAction("Index", "Home");
            }
            transaction.PrintedReceipt = true;
            _appDbContext.SaveChanges();
            return View(new RepeiptViewModel{Transaction = transaction, User = user});
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._appDbContext.Dispose();
                this._appDbContext = null;
            }
            base.Dispose(disposing);
        }

       
    }
}