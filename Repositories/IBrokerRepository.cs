using System.Collections.Generic;
using System.Threading.Tasks;
using StockPortfolioMgmt.Models;

namespace StockPortfolioMgmt.Repositories
{
    public interface IBrokerRepository
    {
        Task<IEnumerable<PortfolioModel>> GetAllCustomersWithBalances();
        Task ImportStockPriceFromCSV(string csvFilePath);
        Task<PortfolioModel> GetPortfolioById(int portfolioId);
        Task<IEnumerable<StockPricesModel>> GetAllStockPrices();

    }
}
