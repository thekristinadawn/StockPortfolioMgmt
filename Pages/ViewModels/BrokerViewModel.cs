using StockPortfolioMgmt.Models;
using System.Collections.Generic;


namespace StockPortfolioMgmt.Pages.ViewModels
{
    public class BrokerViewModel
    {
        public IEnumerable<PortfolioModel> PortfolioWithBalances
        {
            get; set;
        }
        public IEnumerable<StockPricesModel> StockPrices
        {
            get; set;
        }
        public string AdminId
        {
            get; set;
        }
    }
}
