using FileUpload.Models;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
using System.Xml.Linq;

namespace FileUpload.Controllers
{
    public class FileController : ApiController
    {
        [ActionName("GetByCurrency")]
        [HttpGet]
        [ResponseType(typeof(List<TransactionListViewModel>))]
        public IHttpActionResult GetByCurrency([FromUri]string value)
        {
            List<Transaction> searchResult = new List<Transaction>();
            using (var db = new ApplicationDbContext())
            {
                var query = db.Transactions.AsQueryable();
                if (!string.IsNullOrEmpty(value))
                {
                    query = query.Where(x => x.CurrencyCode.ToLower() == value.ToLower());
                }
                searchResult = query.ToList();
            }           
           
            return Ok(searchResult.Select(x => new TransactionListViewModel
            {
                id = x.Code,
                payment = x.Amount + " " + x.CurrencyCode,
                Status = x.OutputStatus
            }).OrderBy(x => x.id).ToList());
        }
        [ActionName("GetByDate")]
        [HttpGet]
        [ResponseType(typeof(List<TransactionListViewModel>))]
        public IHttpActionResult GetByDate([FromUri]string from, string to)
        {
            List<Transaction> searchResult = new List<Transaction>();
            using (var db = new ApplicationDbContext())
            {
                var query = db.Transactions.AsQueryable();
                if (!String.IsNullOrEmpty(from))
                {
                    if (DateTime.TryParseExact(from, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fromDate))
                    {
                        query = query.Where(s => s.TransactionDate >= fromDate);
                    }
                    else
                    {
                        return BadRequest("Invalid from date.");
                    }
                }
                if (!String.IsNullOrEmpty(to))
                {
                    if (DateTime.TryParseExact(to, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime toDate))
                    {
                        query = query.Where(s => s.TransactionDate <= toDate);
                    }
                    else
                    {
                        return BadRequest("Invalid to date.");
                    }
                }
                searchResult = query.ToList();
            }
            return Ok(searchResult.Select(x => new TransactionListViewModel
            {
                id = x.Code,
                payment = x.Amount + " " + x.CurrencyCode,
                Status = x.OutputStatus
            }).OrderBy(x => x.id).ToList());
        }
        [ActionName("GetByStatus")]
        [HttpGet]
        [ResponseType(typeof(List<TransactionListViewModel>))]
        public IHttpActionResult GetByStatus([FromUri]string status)
        {
            List<Transaction> searchResult = new List<Transaction>();
            using (var db = new ApplicationDbContext())
            {
                var query = db.Transactions.AsQueryable();
                if (!string.IsNullOrEmpty(status))
                {
                    Status Status = GetStatusByString(status);
                    query = query.Where(x => x.Status == Status);
                }
                searchResult = query.ToList();
            }
            return Ok(searchResult.Select(x => new TransactionListViewModel
            {
                id = x.Code,
                payment = x.Amount + " " + x.CurrencyCode,
                Status = x.OutputStatus
            }).OrderBy(x => x.id).ToList());
        }
        
        private bool GetModelFromViewModel(bool isCSV, List<TransactionViewModel> viewModelList, out string errorMessage, out List<Transaction> modelList)
        {
            errorMessage = "";
            List<int> invalidRow = new List<int>();
            modelList = new List<Transaction>();
            int i = 1;
            string dateFormat = isCSV ? "dd/MM/yyyy hh:mm:ss" : "yyyy-MM-ddTHH:mm:ss";
            foreach (TransactionViewModel item in viewModelList)
            {
                if (!string.IsNullOrEmpty(item.Code) && !string.IsNullOrEmpty(item.Amount) && !string.IsNullOrEmpty(item.CurrencyCode) && !string.IsNullOrEmpty(item.TransactionDate) && !string.IsNullOrEmpty(item.Status))
                {
                    Status status = GetStatusByString(item.Status);
                    if (decimal.TryParse(item.Amount, out decimal amt) && DateTime.TryParseExact(item.TransactionDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tDate) && status != Status.None)
                    {
                        Transaction model = new Transaction()
                        {
                            Code = item.Code,
                            Amount = amt,
                            CurrencyCode = item.CurrencyCode,
                            TransactionDate = tDate,
                            Status = status,
                            IsCSV = isCSV
                        };
                        modelList.Add(model);
                    }
                    else
                        invalidRow.Add(i);
                }
                else
                    invalidRow.Add(i);

                i++;
            }
            if (invalidRow.Any())
            {
                errorMessage = "Record " + string.Join(", ", invalidRow.ToArray()) + " have invalid data.";
            }
            return string.IsNullOrEmpty(errorMessage);
        }
        // POST api/values
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        List<TransactionViewModel> viewModelList = new List<TransactionViewModel>();
                        TransactionViewModel viewModel = null;
                        bool isCSV = postedFile.FileName.EndsWith(".csv");
                        if (isCSV)
                        {
                            using (CsvReader csv = new CsvReader(new StreamReader(postedFile.InputStream), false))
                            {
                                while (csv.ReadNextRecord())
                                {
                                    if (csv[4].Trim().Trim('"').ToLower() != "status")//check 1st row is header row
                                    {
                                        viewModel = new TransactionViewModel()
                                        {
                                            Code = csv[0].Trim().Trim('"'),
                                            Amount = csv[1].Trim().Trim('"'),
                                            CurrencyCode = csv[2].Trim().Trim('"'),
                                            TransactionDate = csv[3].Trim().Trim('"'),
                                            Status = csv[4].Trim().Trim('"'),
                                            IsCSV = isCSV
                                        };
                                        viewModelList.Add(viewModel);
                                    }

                                }
                            }
                        }
                        else if (postedFile.FileName.EndsWith(".xml"))
                        {
                            using (XmlReader reader = XmlReader.Create(new StreamReader(postedFile.InputStream)))
                            {
                                XDocument xDoc = XDocument.Load(reader);
                                viewModelList = xDoc.Descendants("Transaction").Select
                                    (x => new TransactionViewModel()
                                    {
                                        Code = x.Attribute("id").Value.Trim().Trim('"'),
                                        Amount = x.Element("PaymentDetails").Element("Amount").Value.Trim().Trim('"'),
                                        CurrencyCode = x.Element("PaymentDetails").Element("CurrencyCode").Value.Trim().Trim('"'),
                                        TransactionDate = x.Element("TransactionDate").Value.Trim().Trim('"'),
                                        Status = x.Element("Status").Value.Trim().Trim('"')
                                    }).ToList();
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Unknown format");
                        }
                        if (viewModelList.Any())
                        {
                            if (GetModelFromViewModel(isCSV, viewModelList, out string errorMessage, out List<Transaction> entityList))
                            {
                                using (var db = new ApplicationDbContext())
                                {
                                    foreach (var item in entityList)
                                    {
                                        db.Transactions.Add(item);
                                    }
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, errorMessage);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "No record to save.");
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Please upload file.");
                }
                result = Request.CreateResponse(HttpStatusCode.OK, "Saving Successful.");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Oops! Something happen.");
            }
            return result;
        }
        private Status GetStatusByString(string value)
        {
            Status status = Status.None;

            switch (value)
            {
                case "Approved":
                    status = Status.Approved;
                    break;
                case "Failed":
                    status = Status.Failed;
                    break;
                case "Finished":
                    status = Status.Finished;
                    break;
                case "Rejected":
                    status = Status.Rejected;
                    break;
                case "Done":
                    status = Status.Done;
                    break;

            }

            return status;
        }

    }
}
