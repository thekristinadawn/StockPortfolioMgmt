using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPortfolioMgmt.Models
{
    public class PortfolioModel
    {
        public int PortfolioId { get; set; }
        public int UserId { get; set; }

        [DisplayName("Portfolio Name")]
        public string? PortfolioName { get; set; }

        [DisplayName("Balance")]
        [DataType(DataType.Currency)]
        [Range(0.00,999999999.00, ErrorMessage = "Value for {0} must be between {1:C} and {2:C}")]
        public decimal Balance { get; set; }

        public List<PortfolioStocksModel> Stocks{get; set; } = new List<PortfolioStocksModel>();
    }
}
