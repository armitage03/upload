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
using System.Xml;
using System.Xml.Linq;

namespace FileUpload.Controllers
{
    public class FileController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
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
                        Stream stream = postedFile.InputStream;
                        List<Transaction> entityList = new List<Transaction>();
                        Transaction entity = null;
                        if (postedFile.FileName.EndsWith(".csv"))
                        {
                            using (CsvReader csv = new CsvReader(new StreamReader(stream), false))
                            {                                
                                while (csv.ReadNextRecord())
                                {
                                    entity = new Transaction()
                                    {
                                        Code = csv[0].Trim().Trim('"'),
                                        Amount= Convert.ToDecimal(csv[1].Trim().Trim('"')),
                                        CurrencyCode= csv[2].Trim().Trim('"'),
                                        TransactionDate = DateTime.ParseExact(csv[3].Trim().Trim('"'), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture),
                                        Status= GetStatusByString(csv[4].Trim().Trim('"'))
                                    };
                                    entityList.Add(entity);
                                } 
                            }
                        }
                        else if (postedFile.FileName.EndsWith(".xml"))
                        {
                            using (XmlReader reader = XmlReader.Create(new StreamReader(stream)))
                            {
                                XDocument xDoc = XDocument.Load(reader);
                                entityList = xDoc.Descendants("Transaction").Select
                                    (x => new Transaction()
                                    {
                                        Code = x.Attribute("id").Value.Trim().Trim('"'),
                                        Amount = Convert.ToDecimal(x.Element("PaymentDetails").Element("Amount").Value.Trim().Trim('"')),
                                        CurrencyCode = x.Element("PaymentDetails").Element("CurrencyCode").Value.Trim().Trim('"'),
                                        TransactionDate =DateTime.ParseExact(x.Element("TransactionDate").Value.Trim().Trim('"'),"yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                                        Status = GetStatusByString(x.Element("Status").Value.Trim().Trim('"'))
                                    }).ToList();                               
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        if (entityList.Any())
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
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                result = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
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
        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
