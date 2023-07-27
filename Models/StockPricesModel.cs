using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockPortfolioMgmt.Models
{
    public class StockPricesModel
    {

        public int StockId { get; set; }
        public string? Symbol { get; set; }
        public DateOnly StockDate { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Volume { get; set; }
        public string? Exchange { get; set; }
    }
}
