using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FileUpload.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public bool IsCSV { get; set; }
        [Required]
        public Status Status { get; set; }
        [NotMapped]
        public string OutputStatus
        {
            get
            {
                string status = "";
                if (Status == Status.Approved)
                    status = "A";
                else if (Status == Status.Failed || Status == Status.Rejected)
                    status = "R";
                else if (Status == Status.Finished || Status == Status.Done)
                    status = "D";
                return status;
            }
            private set { }
        }
    }
    public enum Status
    {
        None = 0,
        Approved = 1,
        Failed = 2,
        Finished = 3,
        Rejected = 4,
        Done = 5
    }
}