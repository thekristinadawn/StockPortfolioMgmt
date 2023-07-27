using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace StockPortfolioMgmt.Models
{
    public class TransactionsModel
    {
        public int TransactionId { get; set; }
        public int PortfolioId {  get; set; }   
        public int StockId { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z][A-Za-z ]+$", ErrorMessage = "Please enter name as...")]
        public string? TransactionType { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}\(0[1-9]|1[012])\(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Please enter a date in a valid format: yyyymmdd.")]
        public DateTime TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }  

        public int Quantity { get; set; }

        public decimal Fee { get; set;}

        public TransactionsModel(decimal transactionAmount, string transactionType)
        {
            TransactionAmount = transactionAmount;
            TransactionType = transactionType;
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            switch (TransactionType)
            {
                case "Purchase":
                    Fee = 5.00m; 
                    break;
                case "Sale":
                    Fee = 10.00m; 
                    break;
                default:
                    Fee = 0; // Default to no fee for unknown fee types
                    break;
            }
        }

    }
}
