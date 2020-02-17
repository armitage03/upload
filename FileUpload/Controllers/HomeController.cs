using FileUpload.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Upload Transactions";
            return View();
        }

        public ActionResult Search()
        {
            ViewBag.Title = "Search Transactions";
            using (var db = new ApplicationDbContext())
            {
                List<Transaction> list = db.Transactions.ToList();
                return View(list.Select(x => new TransactionListViewModel
                {
                    id = x.Code,
                    payment = x.Amount + " " + x.CurrencyCode,
                    Status = x.OutputStatus
                }).OrderBy(x => x.id).ToList());
            }
        }
    }
}
