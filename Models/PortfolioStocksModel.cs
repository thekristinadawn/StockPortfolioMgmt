using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolioMgmt.Models
{
    public class PortfolioStocksModel
    {
        public int PortfolioStocksId { get; set; }
        public int StockId { get; set; }
        public string Symbol {get; set; }
        public decimal PurchasePrice { get; set; }
        public DateOnly PurchaseDate { get; set; }
        public int Quantity { get; set; }
    }
}
